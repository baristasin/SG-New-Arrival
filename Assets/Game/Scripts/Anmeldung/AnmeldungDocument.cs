using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Game.Scripts.Anmeldung
{
    public class AnmeldungDocument : MonoBehaviour
    {
        [SerializeField] private Image _paperImage;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private DropSlot[] _slots;

        public DropSlot[] Slots => _slots;

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private Tween _moveTween;
        private Tween _rotateTween;

        private void Awake()
        {
            _originalPosition = _rectTransform.localPosition;
            _originalRotation = _rectTransform.localRotation;
        }

        public void ChangeLanguage(Sprite newPaperSprite)
        {
            _paperImage.sprite = newPaperSprite;
        }

        [Button]
        public void StartShaking(float intensity, float duration)
        {
            _moveTween?.Kill();  
            _moveTween = _rectTransform.DOShakeAnchorPos(duration, intensity, 10, 90, false, true)
                .SetLoops(-1, LoopType.Restart);
        }
        
        public void ShakeOnce(float intensity, float duration)
        {
            _moveTween?.Kill();
            _moveTween = _rectTransform.DOShakeAnchorPos(duration, intensity, 10, 90, false, true);
        }

        [Button]
        public void StartSpinning(float speed)
        {
            _rotateTween?.Kill();  
            _rotateTween = _rectTransform.DOLocalRotate(new Vector3(0, 0, 360f), speed, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }

        public void StopEffects()
        {
            _moveTween?.Kill();
            _rotateTween?.Kill();
        }

        public void Reset()
        {
            StopEffects();
        }
        
        private void OnDestroy()
        {
            _moveTween?.Kill();
            _rotateTween?.Kill();
        }
    }
}