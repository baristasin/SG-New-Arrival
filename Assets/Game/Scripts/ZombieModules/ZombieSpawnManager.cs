using System.Collections;
using Game.Scripts.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public class ZombieSpawnManager : MonoBehaviour
    {
        [SerializeField] private ZombieController _zombiePrefab;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private Vector3 _buildingCenter;
        [SerializeField] private float _buildingHalfWidth = 30f;
        [SerializeField] private int _initialPoolSize = 50;
        [SerializeField] private int _totalGroups = 5;

        private static Pool<ZombieController> _pool;
        private int _spawnCount;

        private void Awake()
        {
            _pool = new Pool<ZombieController>(_zombiePrefab, _initialPoolSize);

            for (int i = 0; i < 1; i++)
            {
                SpawnZombie();
            }

            // StartCoroutine(SpawnZombiesCo());
        }

        private IEnumerator SpawnZombiesCo()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                for (int i = 0; i < 5; i++)
                {
                    SpawnZombie();
                }    
            }
        }

        public static void DespawnZombie(ZombieController zombie)
        {
            _pool.ReturnToPool(zombie);
        }

        [Button]
        public ZombieController SpawnZombie()
        {
            var zombie = _pool.Get();
            var spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

            zombie.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

            int group = _spawnCount % _totalGroups;
            _spawnCount++;

            zombie.Setup(GetRandomBuildingHitPoint(), group, _totalGroups);
            return zombie;
        }

        private Vector3 GetRandomBuildingHitPoint()
        {
            float x = _buildingCenter.x + Random.Range(-_buildingHalfWidth, _buildingHalfWidth);
            return new Vector3(x, _buildingCenter.y, _buildingCenter.z);
        }
    }
}
