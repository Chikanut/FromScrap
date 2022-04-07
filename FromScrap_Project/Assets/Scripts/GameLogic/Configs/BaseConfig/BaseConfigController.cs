using UnityEngine;

namespace Packages.Common.Storage.Config
{
    public class BaseConfigController : IBaseConfigController
    {
        private BaseConfigData _model;
        
        public virtual void SetInfo(BaseConfigScriptable data)
        {
            _model = data.BaseData;
        }
    }
}