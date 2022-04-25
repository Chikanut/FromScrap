using UnityEngine;
using UnityEngine.SubsystemsImplementation;

namespace ArtResources.TrailEffectArt.Scripts
{
    public class TrailEffectTrackingController : MonoBehaviour
    {
        [SerializeField] private GameObject _trailEffectPrefab;
    
        private TrailEffectVisualizationController _trailEffectVisualizationController;
        private Rigidbody _rb;

        public void Init()
        {
            _rb = GetComponent<Rigidbody>();
            _trailEffectPrefab = Instantiate(_trailEffectPrefab, Vector3.zero, Quaternion.identity);
            _trailEffectVisualizationController = _trailEffectPrefab.GetComponent<TrailEffectVisualizationController>();
        }

        private void Update()
        {
            if(_trailEffectVisualizationController == null)
                return;
            
            var forwardVector = (Vector3.Reflect(_rb.velocity, Vector3.up) + _rb.velocity);
            _trailEffectVisualizationController.AddNewPosition(transform.position, Vector3.up, forwardVector.normalized, _rb.velocity.magnitude, transform.localScale.x);
        }

        public void Remove()
        {
            if(_trailEffectVisualizationController == null)
                return;

            var lifeTime = _trailEffectVisualizationController.GetLifeTime();
            
            Destroy(_trailEffectVisualizationController.gameObject, lifeTime);
            Destroy(gameObject, lifeTime);
        }
    }
}
