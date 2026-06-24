using UnityEngine;
using UnityEngine.Splines;

namespace Game.Scripts.Traffic
{
    // One road / loop in the city. Reads a SplineContainer (assigned, or auto-grabbed from this
    // GameObject) and spawns N cars evenly distributed along it at Start. Each car gets its own
    // randomised speed inside _speedRange and a randomly picked prefab. The route lives in the
    // DayCity scene; cars are destroyed with the scene on the next sleep / night transition.
    [DisallowMultipleComponent]
    public class TrafficRoute : MonoBehaviour
    {
        [SerializeField] private SplineContainer _spline;

        [SerializeField] private TrafficCar[] _carPrefabs;

        [SerializeField, Min(0)] private int _carCount = 5;

        [SerializeField] private Vector2 _speedRange = new Vector2(8f, 14f);

        [SerializeField] private bool _loop = true;

        [SerializeField, Range(0f, 1f)] private float _spacingJitter = 0.8f;

        private void Awake()
        {
            if (_spline == null) _spline = GetComponent<SplineContainer>();
        }

        private void Start()
        {
            SpawnCars();
        }

        public void SpawnCars()
        {
            if (_spline == null || _carPrefabs == null || _carPrefabs.Length == 0 || _carCount <= 0)
                return;

            // Stratified random: each car picks a t within its own evenly-sized segment, so the
            // initial gaps vary while two cars can never spawn on top of each other.
            float segment = 1f / _carCount;
            for (int i = 0; i < _carCount; i++)
            {
                float baseT = i * segment;
                float jitter = Random.Range(-0.5f, 0.5f) * _spacingJitter * segment;
                float t = Mathf.Repeat(baseT + jitter, 1f);

                var prefab = _carPrefabs[Random.Range(0, _carPrefabs.Length)];
                if (prefab == null) continue;

                var car = Instantiate(prefab, transform);
                float speed = Random.Range(_speedRange.x, _speedRange.y);
                car.Setup(_spline, t, speed, _loop);
            }
        }
    }
}
