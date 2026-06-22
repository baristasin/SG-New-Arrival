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
        [Tooltip("Wave data per day. Index = day-1 (element 0 → Day 1). Days past the end of " +
                 "the list reuse the LAST entry forever (so Day 5+ keeps running the Day 4 wave).")]
        [UnityEngine.Serialization.FormerlySerializedAs("_waves")]
        [SerializeField] private WaveData[] _wavesByDay;

        private static Pool<ZombieController> _pool;
        private int _spawnCount;

        // Static so any zombie's ZombieDead can bump it without a manager reference. Reset on
        // each scene load in Awake so old kills don't leak between nights.
        public static int KillCount { get; private set; }
        public static void RegisterKill() => KillCount++;

        // Fires once the RunWaves coroutine completes — after this, no more spawns. The night
        // gate uses it to start watching for "0 zombies left" to declare a success.
        public event Action OnAllWavesCompleted;

        private void Awake()
        {
            // Pool is built eagerly; waves are kicked off externally by NightCombatGate once
            // the NightIntro overlay closes — so zombies aren't spawning behind the intro.
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

            // Survival mode: once the scripted wave finishes, re-run the LAST entry forever so
            // pressure keeps climbing until the night clock ends combat. NightCombatGate stops
            // this manager when it ends the night.
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

        // Drops extra zombies onto the field outside the wave schedule. NightCombatGate calls
        // this to penalise loud weapons during Ruhezeit.
        public void SpawnExtra(int count)
        {
            for (int i = 0; i < count; i++) SpawnZombie();
        }

        // Day 1 → element 0, Day 2 → element 1, … Days past the array length stay on the last
        // entry so the night never goes silent.
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
