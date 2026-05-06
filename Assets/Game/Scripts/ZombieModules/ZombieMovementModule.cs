using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.ZombieModules
{
    public enum ZombieAttackTarget
    {
        Player,
        Building
    }
    
    [RequireComponent(typeof(NavMeshAgent))]
    public class ZombieMovementModule : ZombieBaseModule
    {
        [SerializeField] private float _moveSpeed = 2f;

        [SerializeField] private NavMeshAgent _agent;

        private Transform _playerTransform;
        private Vector3 _buildingHitPosition;
        private ZombieAttackTarget _zombieAttackTarget;
        
        public override void Initialize(ZombieController zombieController)
        {
            base.Initialize(zombieController);
            _agent.speed = _moveSpeed;
            _playerTransform = ZombieController.PlayerTransform;
            _buildingHitPosition = ZombieController.BuildingAttackingPosition;
        }

        public void GoToPosition()
        {
            switch (_zombieAttackTarget)
            {
                case ZombieAttackTarget.Player:
                {
                    _agent.Move(_playerTransform.position);
                    break;
                }
                
                case ZombieAttackTarget.Building:
                {
                    _agent.Move(_buildingHitPosition);
                    break;
                }
            }
        }

        public void SetTargetTransformToPlayer()
        {
            _zombieAttackTarget = ZombieAttackTarget.Player;
        }

        public void SetTargetTransformToBuildingHitPoint()
        {
            _zombieAttackTarget = ZombieAttackTarget.Building;
        }
        
        public void Stop() => _agent.ResetPath();
        public bool HasArrived => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
    }
}