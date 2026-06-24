using Game.Scripts.GunModules.Projectiles;
using Game.Scripts.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.GunModules.Turrets
{
    public abstract class ProjectileTurretBase : TurretBase
    {
        [FormerlySerializedAs("_bottlePrefab")]
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private int _poolSize = 8;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private Transform _rotatingHead;
        [SerializeField] private float _turnSpeed = 360f;
        [SerializeField] private float _areaRadius = 0f;

        private Pool<Projectile> _pool;

        protected virtual void Awake()
        {
            if (_projectilePrefab != null)
                _pool = new Pool<Projectile>(_projectilePrefab, _poolSize);
        }

        protected override void OnUpdate()
        {
            if (_rotatingHead == null) return;

            var target = GetNearestTarget();
            if (target == null) return;

            Vector3 dir = target.transform.position - _rotatingHead.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;

            Quaternion look = Quaternion.LookRotation(dir);
            _rotatingHead.rotation = Quaternion.RotateTowards(_rotatingHead.rotation, look, _turnSpeed * Time.deltaTime);
        }

        protected Projectile LaunchAtNearest()
        {
            if (_pool == null) return null;

            var target = GetNearestTarget();
            if (target == null) return null;

            Transform origin = _muzzle != null ? _muzzle : transform;
            var projectile = _pool.Get();
            projectile.transform.SetPositionAndRotation(origin.position, origin.rotation);
            projectile.SetPool(_pool);
            projectile.Launch(target.transform, _data.Damage, _areaRadius, _targetLayerMask);
            return projectile;
        }
    }
}
