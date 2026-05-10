using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _healthText;

        public void UpdateBar(float percentage)
        {
            percentage = Mathf.Clamp01(percentage);
            _fillImage.fillAmount = percentage;
            _healthText.SetText("{0}", (int)(percentage * 100));
        }
    }
}
