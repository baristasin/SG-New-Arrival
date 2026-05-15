using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Game.Scripts.Paperwork
{
    public class PaperDocument : MonoBehaviour
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
            StopEffects();
            _moveTween = _rectTransform.DOShakeAnchorPos(duration, intensity, 10, 90, false, true)
                .SetLoops(-1, LoopType.Restart);
        }

        [Button]
        public void StartRotating(float angle, float speed)
        {
            StopEffects();
            _rotateTween = _rectTransform.DOLocalRotate(new Vector3(0, 0, angle), speed)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        [Button]
        public void StartSpinning(float speed)
        {
            StopEffects();
            _rotateTween = _rectTransform.DOLocalRotate(new Vector3(0, 0, 360f), speed, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }

        public void StopEffects()
        {
            _moveTween?.Kill();
            _rotateTween?.Kill();
            // _rectTransform.localPosition = _originalPosition;
            // _rectTransform.localRotation = _originalRotation;
        }

        public void Reset()
        {
            StopEffects();
        }
    }
}