using Game.Scripts.BuildingModules;
using Game.Scripts.PlayerModules;
using UnityEngine;

namespace Game.Scripts.UI.Screens
{
    // Scene-local night combat HUD root. Lives in the NightCity scene (not under UIManager)
    // because all its bindings — player health module, cathedral health module, the EKG-style
    // player bar, the cathedral bar — are scene objects. Wires the health module events to
    // their respective bars on enable, unwires on disable. BottomGunUI is its own component
    // and is referenced directly by PlayerShootingModule, so it isn't owned here.
    //
    // Note: GameHUD used to do this wiring. It's been folded into NightUI.
    public class NightUI : MonoBehaviour
    {
        [Header("Source modules (scene objects)")]
        [SerializeField] private PlayerHealthModule _playerHealth;
        [SerializeField] private BuildingHealthModule _buildingHealth;

        [Header("Bars")]
        [Tooltip("EKG-style player health bar (PlayerHealthBar.cs).")]
        [SerializeField] private PlayerHealthBar _playerHealthBar;
        [Tooltip("Cathedral / building bar (HealthBarUI.cs).")]
        [SerializeField] private HealthBarUI _buildingHealthBar;

        private void OnEnable()
        {
            if (_playerHealth != null && _playerHealthBar != null)
            {
                _playerHealth.OnHealthChanged += HandlePlayerHealthChanged;
                // Push the current value so the bar reflects the player's starting health
                // without having to wait for the first damage tick.
                _playerHealthBar.SetHealth(_playerHealth.CurrentHealth);
            }
            if (_buildingHealth != null && _buildingHealthBar != null)
            {
                _buildingHealth.OnHealthChanged += _buildingHealthBar.UpdateBar;
                _buildingHealthBar.UpdateBar(_buildingHealth.CurrentHealth);
            }
        }

        private void OnDisable()
        {
            if (_playerHealth != null && _playerHealthBar != null)
                _playerHealth.OnHealthChanged -= HandlePlayerHealthChanged;
            if (_buildingHealth != null && _buildingHealthBar != null)
                _buildingHealth.OnHealthChanged -= _buildingHealthBar.UpdateBar;
        }

        // PlayerHealthModule reports an int; PlayerHealthBar.SetHealth takes a float.
        private void HandlePlayerHealthChanged(int currentHealth)
        {
            _playerHealthBar.SetHealth(currentHealth);
        }
    }
}
