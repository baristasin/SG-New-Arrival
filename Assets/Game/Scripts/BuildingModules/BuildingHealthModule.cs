using System;
using DG.Tweening;
using UnityEngine;
using FMODUnity;

namespace Game.Scripts.BuildingModules
{
    public class BuildingHealthModule : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private int _buildingStartingHealth;
        
        // [SerializeField] private EventReference _buildingDamageEvent;

        public event Action<int> OnHealthChanged;

        public int CurrentHealth => _buildingCurrentHealth;
        public int MaxHealth => _buildingStartingHealth;

        private Color _originalColor;
        private Vector3 _originalScale;
        private Tween _scaleTween;
        private Tween _colorTween;
        private int _buildingCurrentHealth;

        private Color _damageColor = new Color(0.3f, 0.07f, 0.09f, 1f);
        
        private void Awake()
        {
            _originalColor = _renderer.material.color;
            _originalScale = transform.localScale;
            _buildingCurrentHealth = _buildingStartingHealth;
        }

        public void TakeDamage(int damage)
        {
            _buildingCurrentHealth -= damage;
            OnHealthChanged?.Invoke(_buildingCurrentHealth);

            _scaleTween?.Kill();
            _colorTween?.Kill();
            transform.localScale = _originalScale;
            _renderer.material.color = _originalColor;

            _scaleTween = transform.DOPunchScale(_originalScale * 0.08f, 0.3f, 6, 0.5f).SetEase(Ease.OutElastic);
            _colorTween = _renderer.material.DOColor(_damageColor, 0.1f).SetEase(Ease.OutQuad)
                .OnComplete(() => _renderer.material.DOColor(_originalColor, 0.2f).SetEase(Ease.InQuad));

            // AudioManager.Instance.PlayOneShot(_buildingDamageEvent, transform.position);
        }
    }
}
