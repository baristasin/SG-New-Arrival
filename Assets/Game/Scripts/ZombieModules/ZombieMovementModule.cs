using System.Collections;
using Game.Scripts.Utilities;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.ZombieModules
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ZombieMovementModule : ZombieBaseModule
    {
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private NavMeshAgent _agent;

        [SerializeField] private float _flyThreshold = 1f;   // below this: instant nudge, no fly/BT-freeze
        [SerializeField] private float _knockbackDuration = 0.3f;
        [SerializeField] private float _knockbackArcHeight = 0.5f;
        [SerializeField] private AnimationCurve _knockbackCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Vector3 _buildingHitPosition;
        private bool _isKnockedBack;
        private Coroutine _knockbackRoutine;

        public bool IsKnockedBack => _isKnockedBack;

        public override void Initialize(ZombieController zombieController)
        {
            base.Initialize(zombieController);

            // Clean state on (re)spawn — a knockback fly may have been cut short by despawn.
            if (_knockbackRoutine != null) ZombieController.StopCoroutine(_knockbackRoutine);
            _knockbackRoutine = null;
            _isKnockedBack = false;
            _agent.enabled = true;

            _agent.speed = Random.Range(_moveSpeed - 0.8f, _moveSpeed + 0.8f);
            _buildingHitPosition = ZombieController.BuildingAttackingPosition;
        }

        public bool IsCloseEnoughToBeingLured()
        {
            return DistanceUtil.DistanceXZ(PlayerReference.Position, ZombieController.transform.position)
                   < BalanceVariables.Instance.ZombieLureDistance;
        }

        public bool IsCloseEnoughToAttack()
        {
            return DistanceUtil.DistanceXZ(ZombieController.transform.position, GetCurrentAttackTargetPosition())
                   < BalanceVariables.Instance.ZombieAttackDistance;
        }

        [SerializeField] private float _walkThreshold = 1.5f;

        public void GoToPosition()
        {
            if (_isKnockedBack) return;

            ZombieController.ZombieAnimationModule.Play(
                _agent.speed < _walkThreshold ? ZombieAnimState.Walk : ZombieAnimState.Run);

            switch (ZombieController.ZombiePerceptionModule.ZombieAttackTarget)
            {
                case ZombieAttackTarget.Player:
                    _agent.SetDestination(PlayerReference.Position);
                    break;
                case ZombieAttackTarget.Building:
                    _agent.SetDestination(_buildingHitPosition);
                    break;
            }
        }

        public Vector3 GetCurrentAttackTargetPosition()
        {
            switch (ZombieController.ZombiePerceptionModule.ZombieAttackTarget)
            {
                case ZombieAttackTarget.Player:
                    return PlayerReference.Position;
                case ZombieAttackTarget.Building:
                    return _buildingHitPosition;
                default:
                    return Vector3.zero;
            }
        }

        public void Stop()
        {
            ZombieController.ZombieAnimationModule.Play(ZombieAnimState.Idle);
            _agent.ResetPath();
        }

        public void StopMovement()
        {
            _agent.ResetPath();
        }
        
        public void StopForDeath()
        {
            if (!_agent.isActiveAndEnabled || !_agent.isOnNavMesh) return;
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
        }

        public void FaceTarget()
        {
            Vector3 dir = GetCurrentAttackTargetPosition() - ZombieController.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;

            Quaternion target = Quaternion.LookRotation(dir);
            ZombieController.transform.rotation = Quaternion.RotateTowards(
                ZombieController.transform.rotation, target, 360f * Time.deltaTime);
        }

        public void Knockback(Vector3 direction, float distance)
        {
            if (distance < _flyThreshold)
            {
                if (_agent.isActiveAndEnabled && _agent.isOnNavMesh)
                    _agent.Warp(ZombieController.transform.position + direction.normalized * distance);
                return;
            }

            if (_knockbackRoutine != null) ZombieController.StopCoroutine(_knockbackRoutine);
            _knockbackRoutine = ZombieController.StartCoroutine(KnockbackRoutine(direction.normalized, distance));
        }
        
        private IEnumerator KnockbackRoutine(Vector3 dir, float distance)
        {
            _isKnockedBack = true;

            Transform body = ZombieController.transform;
            Vector3 start = body.position;
            Vector3 desired = start + dir * distance;

            Vector3 landing = start;
            if (NavMesh.SamplePosition(desired, out var navHit, distance + 1f, NavMesh.AllAreas))
                landing = navHit.position;

            if (_agent.isActiveAndEnabled && _agent.isOnNavMesh)
                _agent.ResetPath();
            _agent.enabled = false;

            float t = 0f;
            while (t < _knockbackDuration)
            {
                t += Time.deltaTime;
                float u = Mathf.Clamp01(t / _knockbackDuration);
                Vector3 pos = Vector3.Lerp(start, landing, _knockbackCurve.Evaluate(u));
                pos.y += _knockbackArcHeight * Mathf.Sin(u * Mathf.PI);
                body.position = pos;
                yield return null;
            }

            body.position = landing;
            _agent.enabled = true;
            if (_agent.isOnNavMesh) _agent.Warp(landing);

            _isKnockedBack = false;
            _knockbackRoutine = null;
        }

        public bool HasArrived => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
    }
}