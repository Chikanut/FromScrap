using System;
using System.Collections.Generic;

namespace Packages.Common.Storage.Config
{
    [Serializable]
    public class SoundConfigData
    {
        public Dictionary<string, AudioClipConfig> SoundsDictionary = new Dictionary<string, AudioClipConfig>();
    }
}
