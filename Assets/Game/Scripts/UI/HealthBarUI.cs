using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _healthText;

        public void UpdateBar(int currentHealth)
        {
            _healthText.SetText("{0}", currentHealth);
        }
    }
}
