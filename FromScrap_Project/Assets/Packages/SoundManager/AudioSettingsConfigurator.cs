
using ShootCommon.Signals;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Packages.Utils.SoundManager
{
    public class AudioSettingsConfigurator : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioMixer masterMixer;
        [SerializeField] private SoundsConfig soundConfig;

        private ISignalService _signalService;
        
        [Inject]
        public void Init(ISoundController soundController, ISignalService signalService)
        {
            _signalService = signalService;
            soundController.MusicSource = musicSource;
            soundController.MasterMixer = masterMixer;
            soundController.SoundConfig = soundConfig;
            
            DontDestroyOnLoad(musicSource);
        }
    }
}