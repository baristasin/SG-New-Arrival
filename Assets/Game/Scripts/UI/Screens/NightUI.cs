using Game.Scripts.BuildingModules;
using Game.Scripts.Night;
using Game.Scripts.PlayerModules;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Screens
{
    // Scene-local night combat HUD root. Lives in the NightCity scene because its bindings —
    // player + cathedral health modules, the EKG bar, the cathedral bar, the night clock — are
    // all scene objects. Wires their events on enable, unwires on disable.
    public class NightUI : MonoBehaviour
    {
        [Header("Source modules (scene objects)")]
        [SerializeField] private PlayerHealthModule _playerHealth;
        [SerializeField] private BuildingHealthModule _buildingHealth;
        [SerializeField] private NightClock _clock;

        [Header("Bars")]
        [SerializeField] private PlayerHealthBar _playerHealthBar;
        [SerializeField] private HealthBarUI _buildingHealthBar;

        [Header("Survival clock")]
        [SerializeField] private TMP_Text _clockText;
        [Tooltip("Indicator shown while the night is past Ruhezeit start (default 22:00).")]
        [SerializeField] private GameObject _ruhezeitIndicator;
        [SerializeField] private int _ruhezeitStartHour = 22;

        private void OnEnable()
        {
            if (_playerHealth != null && _playerHealthBar != null)
            {
                _playerHealth.OnHealthChanged += HandlePlayerHealthChanged;
                _playerHealthBar.SetHealth(_playerHealth.CurrentHealth);
            }
            if (_buildingHealth != null && _buildingHealthBar != null)
            {
                _buildingHealth.OnHealthChanged += _buildingHealthBar.UpdateBar;
                _buildingHealthBar.UpdateBar(_buildingHealth.CurrentHealth);
            }
            if (_clock != null)
            {
                _clock.OnTimeChanged += HandleTimeChanged;
                HandleTimeChanged(_clock.Hour, _clock.Minute);
            }
            else
            {
                SetRuhezeitVisible(false);
            }
        }

        private void OnDisable()
        {
            if (_playerHealth != null && _playerHealthBar != null)
                _playerHealth.OnHealthChanged -= HandlePlayerHealthChanged;
            if (_buildingHealth != null && _buildingHealthBar != null)
                _buildingHealth.OnHealthChanged -= _buildingHealthBar.UpdateBar;
            if (_clock != null)
                _clock.OnTimeChanged -= HandleTimeChanged;
        }

        private void HandlePlayerHealthChanged(int currentHealth) =>
            _playerHealthBar.SetHealth(currentHealth);

        private void HandleTimeChanged(int hour, int minute)
        {
            if (_clockText != null) _clockText.text = $"{hour:00}:{minute:00}";
            SetRuhezeitVisible(hour >= _ruhezeitStartHour);
        }

        private void SetRuhezeitVisible(bool on)
        {
            if (_ruhezeitIndicator != null) _ruhezeitIndicator.SetActive(on);
        }
    }
}
