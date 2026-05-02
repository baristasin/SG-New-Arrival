using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.ZombieModules
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ZombieMovementModule : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 2f;

        [SerializeField] private NavMeshAgent _agent;

        private void Awake()
        {
            _agent.speed = _moveSpeed;
        }

        public void SetDestination(Vector3 position) => _agent.SetDestination(position);
        public void Stop() => _agent.ResetPath();
        public bool HasArrived => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
    }
}