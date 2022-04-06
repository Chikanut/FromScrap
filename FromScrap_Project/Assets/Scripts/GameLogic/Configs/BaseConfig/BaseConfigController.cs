using UnityEngine;

namespace Packages.Common.Storage.Config
{
    public class BaseConfigController : IBaseConfigController
    {
        private BaseConfigData _model;
        
        public virtual void SetInfo(BaseConfigData data)
        {
            _model = data;
            
            Debug.Log(_model.TestString);
        }
    }
}