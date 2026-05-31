using System;
using System.Collections;
using Game.Scripts.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.ZombieModules
{
    [Serializable]
    public class WaveEntry
    {
        public int ZombieCount;
        public float DelayAfter;
    }

    [Serializable]
    public class WaveData
    {
        public WaveEntry[] Entries;
    }

    public class ZombieSpawnManager : MonoBehaviour
    {
        [SerializeField] private ZombieController _zombiePrefab;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private Vector3 _buildingCenter;
        [SerializeField] private float _buildingHalfWidth = 30f;
        [SerializeField] private int _initialPoolSize = 50;
        [SerializeField] private int _totalGroups = 5;
        [Tooltip("Seconds between each zombie spawned inside a wave entry. Prevents the big " +
                 "single-frame instantiate hitch when an entry has many zombies.")]
        [SerializeField] private float _spawnInterval = 0.08f;
        [SerializeField] private WaveData[] _waves;

        private static Pool<ZombieController> _pool;
        private int _spawnCount;

        private void Awake()
        {
            // Pool is built eagerly; waves are kicked off externally by NightCombatGate once
            // the NightIntro overlay closes — so zombies aren't spawning behind the intro.
            _pool = new Pool<ZombieController>(_zombiePrefab, _initialPoolSize);
        }

        [Button]
        public void StartWaves()
        {
            StartCoroutine(RunWaves());
        }

        private IEnumerator RunWaves()
        {
            foreach (var wave in _waves)
            {
                foreach (var entry in wave.Entries)
                {
                    for (int i = 0; i < entry.ZombieCount; i++)
                    {
                        SpawnZombie();
                        // Drip the spawns out so a 50-zombie entry doesn't hitch the frame.
                        if (i < entry.ZombieCount - 1 && _spawnInterval > 0f)
                            yield return new WaitForSeconds(_spawnInterval);
                    }

                    yield return new WaitForSeconds(entry.DelayAfter);
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
