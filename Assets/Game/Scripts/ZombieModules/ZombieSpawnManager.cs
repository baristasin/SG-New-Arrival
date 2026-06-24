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
        [SerializeField] private float _spawnInterval = 0.08f;
        [SerializeField] private int _maxAliveZombies = 250;
        [UnityEngine.Serialization.FormerlySerializedAs("_waves")]
        [SerializeField] private WaveData[] _wavesByDay;

        private static Pool<ZombieController> _pool;
        private int _spawnCount;

        public static int KillCount { get; private set; }
        public static void RegisterKill() => KillCount++;
        
        public event Action OnAllWavesCompleted;

        private void Awake()
        {
            _pool = new Pool<ZombieController>(_zombiePrefab, _initialPoolSize);
            _spawnCount = 0;
            KillCount = 0;
        }

        [Button]
        public void StartWaves()
        {
            StartCoroutine(RunWaves());
        }

        private IEnumerator RunWaves()
        {
            var wave = GetWaveForDay(GameManager.Instance != null ? GameManager.Instance.CurrentDay : 1);
            if (wave == null || wave.Entries == null || wave.Entries.Length == 0)
            {
                OnAllWavesCompleted?.Invoke();
                yield break;
            }

            foreach (var entry in wave.Entries)
            {
                yield return RunEntry(entry);
            }

            OnAllWavesCompleted?.Invoke();


            var last = wave.Entries[wave.Entries.Length - 1];
            while (true)
            {
                yield return RunEntry(last);
            }
        }

        private IEnumerator RunEntry(WaveEntry entry)
        {
            for (int i = 0; i < entry.ZombieCount; i++)
            {
                SpawnZombie();
                if (i < entry.ZombieCount - 1 && _spawnInterval > 0f)
                    yield return new WaitForSeconds(_spawnInterval);
            }
            yield return new WaitForSeconds(entry.DelayAfter);
        }


        public void SpawnExtra(int count)
        {
            for (int i = 0; i < count; i++) SpawnZombie();
        }


        private WaveData GetWaveForDay(int day)
        {
            if (_wavesByDay == null || _wavesByDay.Length == 0) return null;
            int idx = Mathf.Clamp(day - 1, 0, _wavesByDay.Length - 1);
            return _wavesByDay[idx];
        }

        public static void DespawnZombie(ZombieController zombie)
        {
            _pool.ReturnToPool(zombie);
        }

        [Button]
        public ZombieController SpawnZombie()
        {
            // Hard cap on alive zombies so the field never gets above _maxAliveZombies — keeps
            // the framerate sane and stops late-night waves from snowballing forever.
            if (ZombieRegistry.Count >= _maxAliveZombies) return null;

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
