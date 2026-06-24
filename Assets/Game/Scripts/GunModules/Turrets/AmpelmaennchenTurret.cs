using FMOD.Studio;
using FMODUnity;
using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.GunModules.Turrets
{

    public class AmpelmaennchenTurret : TurretBase
    {
        [SerializeField] private float _minPhase = 5f;
        [SerializeField] private float _maxPhase = 6f;
        [SerializeField] private bool _danceOnGreen = false;   // false = red is the dance phase
        [SerializeField] private GameObject _redLight;
        [SerializeField] private GameObject _greenLight;

        [SerializeField] private Vector3 _areaOffset = new Vector3(0f, 0f, 5f);
        [SerializeField] private Vector3 _areaHalfExtents = new Vector3(3f, 1.5f, 5f);
        [SerializeField] private float _danceRefresh = 0.25f;

        [SerializeField] private EventReference _moveEvent;
        [SerializeField] private EventReference _waitLoopEvent;

        private bool _isGreen;
        private float _phaseTimer;
        private readonly Collider[] _buffer = new Collider[32];
        private EventInstance _waitInstance;

        private void Start()
        {
            SetGreen(false);   // always starts red
        }

        private void OnDisable()
        {
            if (_waitInstance.isValid()) AudioManager.Instance.Stop(ref _waitInstance);
        }

        protected override void OnUpdate()
        {
            _phaseTimer -= Time.deltaTime;
            if (_phaseTimer <= 0f)
                SetGreen(!_isGreen);

            if (_isGreen == _danceOnGreen)
                ApplyDanceInArea();
        }

        private void SetGreen(bool green)
        {
            _isGreen = green;
            _phaseTimer = Random.Range(_minPhase, _maxPhase);
            if (_redLight != null) _redLight.SetActive(!green);
            if (_greenLight != null) _greenLight.SetActive(green);

            // Click on every transition.
            if (!_moveEvent.IsNull) AudioManager.Instance.PlayOneShot(_moveEvent, transform.position);

            // Wait loop runs during the dance phase.
            bool dancePhase = _isGreen == _danceOnGreen;
            if (dancePhase && !_waitLoopEvent.IsNull && !_waitInstance.isValid())
                _waitInstance = AudioManager.Instance.PlayLoopAttached(_waitLoopEvent, gameObject);
            else if (!dancePhase && _waitInstance.isValid())
                AudioManager.Instance.Stop(ref _waitInstance);
        }

        private void ApplyDanceInArea()
        {
            Vector3 center = transform.TransformPoint(_areaOffset);
            int count = Physics.OverlapBoxNonAlloc(center, _areaHalfExtents, _buffer, transform.rotation, _targetLayerMask);
            for (int i = 0; i < count; i++)
            {
                if (ZombieRegistry.TryGetHealth(_buffer[i], out var health))
                    health.ZombieController.Dance(_danceRefresh);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(_areaOffset), transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _areaHalfExtents * 2f);
        }
    }
}
