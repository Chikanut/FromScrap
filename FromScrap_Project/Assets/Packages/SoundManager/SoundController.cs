using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using Packages.Common.Storage.Config;
using Packages.Utils.SoundManager.Signals;
using ShootCommon.Signals;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using Visartech.Progress;
using Zenject;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Packages.Utils.SoundManager
{
    public class SoundController : ISoundController, IInitializable
    {
        public class ClipInfo {
            public AudioClip Clip;
            public float Volume;
            public AudioMixerGroup OutputAudioMixerGroup;
            public Action<AudioSource> SourceCallback;
            public int MaxSimultaneouslyCalls = 4;
            public Vector3? Position;
        }
        
        public static SoundController Instance;
        
        private AudioSource _musicSource;
        private AudioMixer _masterMixer;

        private string _currentMusicClip;
        public string CurrentMusicClip => _currentMusicClip;
        
        private ISoundConfigController _soundsConfig;
        public ISoundConfigController SoundConfig
        {
            set => _soundsConfig = value; 
        }

        private readonly List<AudioSource> _activeAudioSources;
        
        private string musicClipNow;
        private bool _soundOn = true;
        private bool _musicOn = true;
        private bool _startMusicTrack;
        private float _volume;
        
        private AudioSource _clipInstance;
        private ISignalService _signalService;

        [Inject]
        public void Init(ISignalService signalService,
            ISoundConfigController soundConfigController)
        {
            _signalService = signalService;
            _soundsConfig = soundConfigController;
            
            CreateListeners();
            
            Instance = this;
        }
        
        public void Initialize()
        {
            UpdateSettings();
        }
        
        public AudioSource MusicSource
        {
            set => _musicSource = value;
        }

        public AudioMixer MasterMixer
        {
            set
            {
                _masterMixer = value;
                EffectsOutputGroup = _masterMixer.FindMatchingGroups("EffectsMixer")[0];
                MusicOutputGroup = _masterMixer.FindMatchingGroups("MusicMixer")[0];
            }
        }
        
        public bool SoundOn {
            get => _soundOn;
            set {
                if(_masterMixer == null)
                    return;
                
                _soundOn = value;
                _masterMixer.SetFloat("SoundVolume", value ? 0f : -60f);
            }
        }
        
        public bool MusicOn {
            get => _musicOn;
            set {
                if(_masterMixer == null)
                    return;
                
                _musicOn = value;
                _masterMixer.SetFloat("MusicVolume", value ? 0f : -60f);
            }
        }
        
        public float Volume {
            get => _volume;
            set {
                if(_masterMixer == null)
                    return;
                
                _volume = value;
                _masterMixer.SetFloat("MasterVolume", value);
            }

        }

        public AudioMixerGroup EffectsOutputGroup { get; private set; }
        public AudioMixerGroup MusicOutputGroup { get; private set; }

        public SoundController()
        {
            _activeAudioSources = new List<AudioSource>();
        }

        public string GetMusicClipNow()
        {
            return musicClipNow;
        }

        private void CreateListeners()
        {
            _signalService.Receive<PlayAudioClipAddressableSignal>()
                .Subscribe(PlayAddressable);
            _signalService.Receive<StopAudioClipAddressableSignal>()
                .Subscribe(StopAddressable);
        }

        public void StopAddressable(StopAudioClipAddressableSignal obj)
        {
            Delete2DAudioClip(obj.ClipSource);
        }

        public void PlayAddressable(PlayAudioClipAddressableSignal signal)
        {
            if (signal.ClipType == SoundType.Music)
            {
                if (signal.ClipName != musicClipNow)
                    _startMusicTrack = false;
                
                musicClipNow = signal.ClipName;
                PlayAudioClipAddressable(signal);
            }
            else
            {
                PlayAudioClipAddressable(signal);
            }
        }

        private void UpdateSettings()
        {
            MusicOn = Progress.Game.MusicEnabled;
            SoundOn = Progress.Game.SoundEnabled;
            PushMusic();
        }
        
        private void PushMusic()
        {
            string musicClipNow = GetMusicClipNow();
            if(musicClipNow == null)
                return;
            
            PlayAddressable(new PlayAudioClipAddressableSignal()
            {
                ClipName = musicClipNow,
                ClipType = SoundType.Music
            });
        }

        public void SetMusicLowPassCutoff(bool isActive) {
            if(_masterMixer == null)
                return;
            
            _masterMixer.DOSetFloat("LowPassCutoff", isActive ? 1400 : 22000, 0.3f).SetEase(Ease.OutCubic);
        }

        #region PlayAudioClip Addressable

        public void PlayAudioClipAddressable(PlayAudioClipAddressableSignal clipInfo)
        {
            if (_soundsConfig == null) return;
         
            if (!_soundsConfig.GetSoundData.SoundsDictionary.TryGetValue(clipInfo.ClipName, out var clip))
            {
                Debug.LogError($"No clip config found for {clipInfo.ClipName} audio clip type");
                return;
            }

            if (clip.Reference == null)
                return;

            CreateAddressableAudioClip(clip, clipInfo, info =>
            {
                switch (clipInfo.ClipType)
                {
                    case SoundType.Default:
                        PlayAudioClip(info, clipInfo.Pitch);
                        break;
                    case SoundType.Loop:
                        PlayAudioClipLoop(info, clipInfo.Pitch, clipInfo.PitchTime);
                        break;
                    case SoundType.Singleton:
                        if (clipInfo.Delay > 0)
                            PlayAudioClipSingleton(info, clipInfo.Delay);
                        else
                            PlayAudioClipSingleton(info);
                        break;
                    case SoundType.Delayed:
                        PlayAudioClipDelayed(info, clipInfo.Delay);
                        break;
                    case SoundType.Music:
                        if(_startMusicTrack) return;
            
                        _startMusicTrack = true;
                        PlayMusic(info);
                        break;
                    default:
                        PlayAudioClip(info);
                        break;
                }
            });

        }

        private void CreateAddressableAudioClip(AudioClipConfig clipConfig, PlayAudioClipAddressableSignal clipInfo, Action<ClipInfo> callback)
        {
            AssetReference reference = clipConfig.GetReference();
            var config = new ClipInfo
            {
                Volume = clipConfig.Volume,
                OutputAudioMixerGroup = clipConfig.OutputAudioMixerGroup,
                SourceCallback = clipInfo.Source,
                MaxSimultaneouslyCalls = clipConfig.MaxSimultaneouslyCalls,
                Position = clipInfo.Position
            };

            if (reference.OperationHandle.IsValid())
            {
                if (!reference.OperationHandle.IsDone)
                {
                    reference.OperationHandle.Completed += (loadRequest) =>
                    {
                        AudioClip clip = loadRequest.Result as AudioClip;
                        config.Clip = clip;
                        callback?.Invoke(config);
                    };
                }
                else
                {
                    AudioClip clip = reference.OperationHandle.Result as AudioClip;
                    config.Clip = clip;
                    callback?.Invoke(config);
                }
            }
            else
            {
                reference.LoadAssetAsync<AudioClip>().Completed += (loadRequest) =>
                {
                    AudioClip clip = loadRequest.Result;
                    config.Clip = clip;
                    callback?.Invoke(config);
                };
            }
        }
        
        #endregion

        void PlayAudioClip(ClipInfo clip, float pitch = 1)
        {
            if (!SoundOn) return;
     
            if (clip.Clip == null)
                return;

            var soundsPlaying = _activeAudioSources.Count(a => a != null && a.clip == clip.Clip);
            if(soundsPlaying >= clip.MaxSimultaneouslyCalls) return;

            CreateAndPlayAudioClip(clip, false, false, pitch);
        }

        void PlayAudioClipLoop(ClipInfo clip, float pitch = 1f, float pitchTime = 0.2f) {
            if (!SoundOn) return;
     
            if (clip.Clip == null)
                return;
            int index = _activeAudioSources.FindIndex(audio => audio != null && audio.clip == clip.Clip);

            if (index != -1) {
                if (pitch != _activeAudioSources[index].pitch)
                {
                    _activeAudioSources[index].DOPitch(pitch, pitchTime);
                    clip.SourceCallback?.Invoke(_activeAudioSources[index]);
                }
            }
            else {
                CreateAndPlayAudioClip(clip, false, true, pitch);
            }
        }
        
        void PlayAudioClipSingleton(ClipInfo clip) {
            if (!SoundOn) return;

            if (clip.Clip == null)
                return;

            if (_clipInstance != null) {
                _activeAudioSources.Remove(_clipInstance);
                Object.Destroy(_clipInstance.gameObject);
            }

            _clipInstance = CreateAndPlayAudioClip(clip, true);
        }

        void PlayAudioClipSingleton(ClipInfo clip, float delay) {
            if (!SoundOn || clip.Clip == null) return;
            Timing.RunCoroutine(PlayAudioClipSingletonEnum(clip, delay));
        }

        private IEnumerator<float> PlayAudioClipSingletonEnum(ClipInfo clip, float delay) {
            yield return Timing.WaitForSeconds(delay);

            if (_clipInstance != null) {
                _activeAudioSources.Remove(_clipInstance);
                Object.Destroy(_clipInstance.gameObject);
            }

            _clipInstance = CreateAndPlayAudioClip(clip, true);
        }
        
        private AudioSource CreateAndPlayAudioClip(ClipInfo clipConfig, bool cancelWithGo = false,
            bool isOnLoop = false, float pitch = 1f)
        {
            var audioSource = new GameObject($"{clipConfig.Clip.name}").AddComponent<AudioSource>();
            
            audioSource.playOnAwake = false;
            audioSource.loop = isOnLoop;
            
            if(clipConfig.Position == null)
                audioSource.spatialize = false;
            else
            {
                audioSource.transform.position = clipConfig.Position.Value;
                audioSource.spatialBlend = 1;
            }

            audioSource.clip = clipConfig.Clip;
            audioSource.volume = clipConfig.Volume;
            audioSource.outputAudioMixerGroup = clipConfig.OutputAudioMixerGroup;
            audioSource.pitch = pitch;
            audioSource.Play();

            _activeAudioSources.Add(audioSource);
            
            Timing.RunCoroutine(cancelWithGo
                ? DestroyClipOnFinish(audioSource, clipConfig).CancelWith(audioSource.gameObject)
                : DestroyClipOnFinish(audioSource, clipConfig));
            
            clipConfig.SourceCallback?.Invoke(audioSource);
            return audioSource;
        }

        private void Delete2DAudioClip(AudioSource source) {
            if ( source == null || _activeAudioSources == null || _activeAudioSources.Count == 0)
                return;

            if (!_activeAudioSources.Contains(source))
                return;
            
            _activeAudioSources.Remove(source);
            Object.Destroy(source.gameObject);
        }

        private IEnumerator<float> DestroyClipOnFinish(AudioSource audioSource, ClipInfo clipConfig) {
            yield return Timing.WaitForSeconds(clipConfig.Clip.length);
            if (audioSource == null) yield break;
            if (!audioSource.isPlaying) {
                _activeAudioSources.Remove(audioSource);
                Object.Destroy(audioSource.gameObject);
            }
            else {
                while (audioSource != null && audioSource.isPlaying) {
                    yield return Timing.WaitForOneFrame;
                }

                if (audioSource == null) yield break;
                _activeAudioSources.Remove(audioSource);
                Object.Destroy(audioSource.gameObject);
            }
        }
        

        public void PlayAudioClipDelayed(ClipInfo clip, float delay) {
            if (!SoundOn || clip == null || clip.Clip == null) return;
            Timing.RunCoroutine(PlayAudioClipDelayedCo(clip, delay));
        }

        private IEnumerator<float> PlayAudioClipDelayedCo(ClipInfo clipConfig, float delay) {
            yield return Timing.WaitForSeconds(delay);
            CreateAndPlayAudioClip(clipConfig);
        }
        

        public void PlayMusic(ClipInfo clip)
        {
            _musicSource.Stop();
            _musicSource.clip = clip.Clip;
            _musicSource.volume = clip.Volume;
            _musicSource.outputAudioMixerGroup = clip.OutputAudioMixerGroup;
            _musicSource.loop = true;
            _currentMusicClip = clip.Clip.name;
            _musicSource.Play();
        }

        public void StopMusic() {
            _musicSource.Stop();

            _musicSource.clip = null;
        }

        public void DestroyActiveAudioSources() {
            for (int i = 0; i < _activeAudioSources.Count; i++) {
                var source = _activeAudioSources[i];
                if (source != null) {
                    Object.Destroy(source.gameObject);
                }
            }

            _activeAudioSources.Clear();
        }
    }
}