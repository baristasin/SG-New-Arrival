using Game.Scripts.Utilities;
using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.GunModules.Projectiles
{

    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 12f;
        [SerializeField] private float _turnSpeed = 360f;   // deg/sec homing steering
        [SerializeField] private float _hitRadius = 0.35f;
        [SerializeField] private float _lockReleaseDistance = 1.5f;   // commit & fly straight once this close
        [SerializeField] private float _lifeTime = 5f;
        [SerializeField] private GameObject _hitEffect;

        private Transform _target;
        private Vector3 _direction;
        private int _damage;
        private float _areaRadius;
        private LayerMask _targetLayerMask;
        private float _age;
        private bool _consumed;
        private Pool<Projectile> _pool;

        private readonly Collider[] _buffer = new Collider[16];

        public void SetPool(Pool<Projectile> pool) => _pool = pool;

        public void Launch(Transform target, int damage, float areaRadius, LayerMask targetLayerMask)
        {
            _target = target;
            _damage = damage;
            _areaRadius = areaRadius;
            _targetLayerMask = targetLayerMask;
            _age = 0f;
            _consumed = false;

            Vector3 toTarget = target != null ? target.position - transform.position : transform.forward;
            _direction = toTarget.sqrMagnitude > 0.0001f ? toTarget.normalized : transform.forward;
            transform.forward = _direction;
        }

        private void Update()
        {
            _age += Time.deltaTime;
            if (_age >= _lifeTime) { Despawn(); return; }

            if (_target != null && !_target.gameObject.activeInHierarchy)
                _target = null;

            if (_target != null)
            {
                Vector3 desired = _target.position - transform.position;


                if (desired.sqrMagnitude <= _lockReleaseDistance * _lockReleaseDistance)
                    _target = null;
                else
                    _direction = Vector3.RotateTowards(_direction, desired.normalized,
                        _turnSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f).normalized;
            }

            transform.position += _direction * (_speed * Time.deltaTime);
            transform.forward = _direction;

            TryHit();
        }

        private void TryHit()
        {
            if (Physics.OverlapSphereNonAlloc(transform.position, _hitRadius, _buffer, _targetLayerMask) == 0)
                return;

            float radius = Mathf.Max(_hitRadius, _areaRadius);
            int count = Physics.OverlapSphereNonAlloc(transform.position, radius, _buffer, _targetLayerMask);
            for (int i = 0; i < count; i++)
            {
                if (ZombieRegistry.TryGetHealth(_buffer[i], out var health))
                    health.GetHit(_damage);
            }

            Despawn();
        }

        private void Despawn()
        {
            if (_consumed) return;
            _consumed = true;
            if (_hitEffect != null) Instantiate(_hitEffect, transform.position, Quaternion.identity);

            if (_pool != null) _pool.ReturnToPool(this);
            else Destroy(gameObject);
        }
    }
}
