using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private int _maxHealth;

        public void UpdateBar(int currentHealth)
        {
            int clamped = Mathf.Max(0, currentHealth);
            _fillImage.fillAmount = _maxHealth > 0 ? (float)clamped / _maxHealth : 0f;
        }
    }
}
