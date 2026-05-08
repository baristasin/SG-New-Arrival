using Game.Scripts.BuildingModules;
using Game.Scripts.PlayerModules;
using UnityEngine;

namespace Game.Scripts.Utilities
{
    public class AttackProcessorRunner : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private PlayerHealthModule _playerHealth;
        [SerializeField] private BuildingHealthModule _buildingHealth;
        [SerializeField] private float _dodgeThreshold = 4f;

        private void Awake()
        {
            PlayerReference.Set(_playerTransform);
        }

        private void LateUpdate()
        {
            AttackProcessor.ProcessQueue(_playerHealth, _buildingHealth, _dodgeThreshold);
        }
    }
}
