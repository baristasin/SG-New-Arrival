using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.SanityModules
{
    // Drives a Filled Image from the SanityManager's normalized value.
    public class SanityBar : MonoBehaviour
    {
        [SerializeField] private Image _fill;   // Image, Image Type = Filled

        private SanityManager _manager;

        private void OnEnable()
        {
            _manager = SanityManager.Instance;
            _manager.OnSanityChanged += UpdateBar;
            UpdateBar(_manager.Normalized);
        }

        private void OnDisable()
        {
            if (_manager != null)
                _manager.OnSanityChanged -= UpdateBar;
        }

        private void UpdateBar(float normalized)
        {
            if (_fill != null) _fill.fillAmount = normalized;
        }
    }
}
