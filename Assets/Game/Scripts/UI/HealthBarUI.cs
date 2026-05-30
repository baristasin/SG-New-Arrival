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
            // Clamp at 0 instead of early-returning so the bar correctly reads empty when the
            // health bottoms out (otherwise the last positive value stays on screen forever).
            int clamped = Mathf.Max(0, currentHealth);
            _fillImage.fillAmount = _maxHealth > 0 ? (float)clamped / _maxHealth : 0f;
        }
    }
}
