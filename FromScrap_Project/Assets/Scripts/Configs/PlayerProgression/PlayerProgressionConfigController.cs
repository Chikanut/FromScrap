using UnityEngine;
using System;
using System.Collections.Generic;

namespace Packages.Common.Storage.Config
{
    [Serializable]
    public class PlayerProgressionConfigData 
    {
        public List<int> LevelsExperience = new List<int>();
    }
}

namespace Packages.Common.Storage.Config
{
    public interface IPlayerProgressionConfigController 
    {
        PlayerProgressionConfigData GetPlayerProgressionData { get; }
        void SetInfo(PlayerProgressionConfigScriptable data);
    }
}

namespace Packages.Common.Storage.Config
{
    public class PlayerProgressionConfigController : IPlayerProgressionConfigController
    {
        private PlayerProgressionConfigData _model;

        public PlayerProgressionConfigData GetPlayerProgressionData => _model;
        
        public virtual void SetInfo(PlayerProgressionConfigScriptable data)
        {
            _model = new PlayerProgressionConfigData
            {
                LevelsExperience = data.PlayerLevels.LevelsExperience
            };
        }
    }
}