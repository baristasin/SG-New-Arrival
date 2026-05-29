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
            if (currentHealth <= 0)
            {
                return;
            }
            
            _fillImage.fillAmount = (float)currentHealth / _maxHealth;
        }
    }
}
