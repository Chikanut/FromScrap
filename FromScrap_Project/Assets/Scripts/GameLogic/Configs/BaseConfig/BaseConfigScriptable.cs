using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.Common.Storage.Config
{
    [CreateAssetMenu(fileName = "BaseConfig", menuName = "Configs/BaseConfig", order = 1)]
    public class BaseConfigScriptable : ScriptableObject
    {
        [SerializeField] public BaseConfigData BaseData;
    }
}
