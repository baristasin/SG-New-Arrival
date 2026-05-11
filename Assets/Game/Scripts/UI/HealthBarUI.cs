using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private int _maxHealth;

        public void UpdateBar(int currentHealth)
        {
            _fillImage.fillAmount = (float)currentHealth / _maxHealth;
            _healthText.SetText("{0}", currentHealth);
        }
    }
}
