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

        private Vector3 _buildingHitPosition;

        public override void Initialize(ZombieController zombieController)
        {
            base.Initialize(zombieController);
            _agent.speed = Random.Range(_moveSpeed - 0.5f, _moveSpeed + 0.5f);
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

        public void GoToPosition()
        {
            ZombieController.ZombieAnimationModule.Play(ZombieAnimState.Run);

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
            _agent.Warp(ZombieController.transform.position + direction * distance);
        }

        public bool HasArrived => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
    }
}