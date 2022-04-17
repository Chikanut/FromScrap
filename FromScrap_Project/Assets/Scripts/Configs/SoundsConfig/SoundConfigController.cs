using System.Collections.Generic;

namespace Packages.Common.Storage.Config
{
    public class SoundConfigController : ISoundConfigController
    {
        private SoundConfigData _model;

        public SoundConfigData GetSoundData => _model;
        
        public virtual void SetInfo(SoundsConfig data)
        {
            _model = new SoundConfigData();
            _model.SoundsDictionary = new Dictionary<string, AudioClipConfig>();

            foreach (var clipJKey in data.ClipsDictionary.Keys)
            {
                _model.SoundsDictionary.Add(clipJKey, data.ClipsDictionary[clipJKey]);
            }
        }
    }
}