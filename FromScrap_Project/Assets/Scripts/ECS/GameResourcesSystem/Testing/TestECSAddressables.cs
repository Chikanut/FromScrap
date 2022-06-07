using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ECS.LevelSpawnerSystem
{
    public class TestECSAddressables : MonoBehaviour
    {
        [SerializeField] private AssetReference _testGOLoad;
        public int ElementsCount;
        public bool EnableSpawn = false;
        public float SpawningTime = 0f;

        private float _lastSpawnTime = 0f;

        private void Update()
        {
            if(!EnableSpawn)
                return;

            EnableSpawn = false;
            
            SpawningTime = 0f;
            _lastSpawnTime = Time.time;
            
            for (var i = 0; i < ElementsCount; i++)
            {
                StartSpawn(i);
            }
        }

        private async void StartSpawn(int i)
        {
            AsyncOperationHandle<GameObject> handle = _testGOLoad.InstantiateAsync();
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var go = handle.Result;
               
                go.transform.position = new Vector3(i, 0f, 0f);

                var deltaTime = Time.time - _lastSpawnTime;

                SpawningTime += deltaTime;
                _lastSpawnTime = Time.time;

                //_testGOLoad.ReleaseInstance(go);
                //Addressables.Release(handle);
                //testGOLoad.ReleaseAsset();
            }
        }
    }
}