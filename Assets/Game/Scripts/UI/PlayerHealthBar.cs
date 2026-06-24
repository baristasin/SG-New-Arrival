using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    // EKG-style player health bar: heart-rate images scroll left inside a mask, the leftmost wraps
    // to the right end. As health drops the scroll speeds up and the images shrink. The health text
    // shows the current value and steps through green → yellow → orange → red at 75/50/25.
    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _startingHealth = 100f;
        [SerializeField] private TextMeshProUGUI _healthText;

        [SerializeField] private Color _greenColor  = new Color(0.20f, 0.85f, 0.20f);
        [SerializeField] private Color _yellowColor = new Color(0.95f, 0.90f, 0.20f);
        [SerializeField] private Color _orangeColor = new Color(1.00f, 0.55f, 0.10f);
        [SerializeField] private Color _redColor    = new Color(0.95f, 0.20f, 0.15f);

        [SerializeField] private RectTransform _mask;
        [SerializeField] private RectTransform[] _heartImages;
        [SerializeField] private float _imageSpacing = 0f;

        [SerializeField] private Vector2 _speedRange = new Vector2(60f, 240f);
        [SerializeField] private Vector2 _scaleRange = new Vector2(0.4f, 1f);

        [SerializeField] private float _pulseAmplitude = 0.15f;
        [SerializeField] private Vector2 _pulseFrequencyRange = new Vector2(0.6f, 2.5f);

        private float _currentHealth;
        private float _imageWidth;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public float Normalized => _maxHealth > 0f ? Mathf.Clamp01(_currentHealth / _maxHealth) : 0f;

        private void Awake()
        {
            if (_heartImages != null && _heartImages.Length > 0 && _heartImages[0] != null)
                _imageWidth = _heartImages[0].rect.width;

            SetHealth(_startingHealth);
        }

        private void Update()
        {
            float n = Normalized;
            PulseHealthNumber(n);

            if (_heartImages == null || _heartImages.Length == 0 || _mask == null) return;

            float speed = Mathf.Lerp(_speedRange.y, _speedRange.x, n);   // low health → fast
            float scale = Mathf.Lerp(_scaleRange.x, _scaleRange.y, n);   // low health → short on Y only

            // X scale stays at 1, so the on-screen horizontal extent of an image never changes.
            // Use the unscaled width for both the wrap boundary and the wrap-placement distance —
            // otherwise low Y scale would shrink horizontal spacing and the images would crowd.
            float leftBoundary = -_mask.rect.width * 0.5f - _imageWidth * 0.5f;

            float dx = speed * Time.deltaTime;

            // Scroll everything left + apply scale.
            for (int i = 0; i < _heartImages.Length; i++)
            {
                var rt = _heartImages[i];
                if (rt == null) continue;
                rt.localScale = new Vector3(1f, scale, 1f);
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - dx, rt.anchoredPosition.y);
            }

            // Wrap any image that went past the left boundary to the right of the rightmost one.
            for (int i = 0; i < _heartImages.Length; i++)
            {
                var rt = _heartImages[i];
                if (rt == null || rt.anchoredPosition.x >= leftBoundary) continue;

                float rightmostX = float.NegativeInfinity;
                for (int j = 0; j < _heartImages.Length; j++)
                {
                    if (j == i || _heartImages[j] == null) continue;
                    if (_heartImages[j].anchoredPosition.x > rightmostX)
                        rightmostX = _heartImages[j].anchoredPosition.x;
                }
                if (rightmostX > float.NegativeInfinity)
                    rt.anchoredPosition = new Vector2(rightmostX + _imageWidth + _imageSpacing, rt.anchoredPosition.y);
            }
        }

        public void SetHealth(float value)
        {
            _currentHealth = Mathf.Clamp(value, 0f, _maxHealth);
            UpdateHealthText();
        }

        public void TakeDamage(float damage) => SetHealth(_currentHealth - damage);
        public void Heal(float amount)       => SetHealth(_currentHealth + amount);

        // Sine pulse on the health number — frequency lerps from slow (full health) to fast
        // (zero health), so it visibly speeds up as the number drops. Amplitude stays constant.
        private void PulseHealthNumber(float normalized)
        {
            if (_healthText == null || _pulseAmplitude <= 0f) return;

            float hz = Mathf.Lerp(_pulseFrequencyRange.y, _pulseFrequencyRange.x, normalized);
            float wave = Mathf.Sin(Time.time * hz * Mathf.PI * 2f) * 0.5f + 0.5f;   // 0..1
            float s = 1f + wave * _pulseAmplitude;
            _healthText.rectTransform.localScale = new Vector3(s, s, 1f);
        }

        private void UpdateHealthText()
        {
            if (_healthText == null) return;
            _healthText.text = Mathf.RoundToInt(_currentHealth).ToString();
            _healthText.color = GetHealthColor();
        }

        private Color GetHealthColor()
        {
            float ratio = Normalized;
            if (ratio >= 0.75f) return _greenColor;
            if (ratio >= 0.50f) return _yellowColor;
            if (ratio >= 0.25f) return _orangeColor;
            return _redColor;
        }

        [Button] private void DebugDamage10() => TakeDamage(10f);
        [Button] private void DebugHeal10()   => Heal(10f);
        [Button] private void DebugRefill()   => SetHealth(_maxHealth);
    }
}
