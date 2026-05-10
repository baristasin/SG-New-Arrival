using Game.Scripts.BuildingModules;
using Game.Scripts.PlayerModules;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private HealthBarUI _playerHealthBar;
        [SerializeField] private HealthBarUI _buildingHealthBar;
        [SerializeField] private PlayerHealthModule _playerHealth;
        [SerializeField] private BuildingHealthModule _buildingHealth;

        private void OnEnable()
        {
            _playerHealth.OnHealthChanged += _playerHealthBar.UpdateBar;
            _buildingHealth.OnHealthChanged += _buildingHealthBar.UpdateBar;
        }

        private void OnDisable()
        {
            _playerHealth.OnHealthChanged -= _playerHealthBar.UpdateBar;
            _buildingHealth.OnHealthChanged -= _buildingHealthBar.UpdateBar;
        }
    }
}
