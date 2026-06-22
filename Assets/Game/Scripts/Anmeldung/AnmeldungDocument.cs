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
            _moveTween?.Kill();   // only kill the position tween — leaves rotation alone
            _moveTween = _rectTransform.DOShakeAnchorPos(duration, intensity, 10, 90, false, true)
                .SetLoops(-1, LoopType.Restart);
        }

        // One-shot shake (no loop) that doesn't kill the rotation — used for the periodic tremble
        // at Critical sanity while the paper is spinning.
        public void ShakeOnce(float intensity, float duration)
        {
            _moveTween?.Kill();
            _moveTween = _rectTransform.DOShakeAnchorPos(duration, intensity, 10, 90, false, true);
        }

        [Button]
        public void StartSpinning(float speed)
        {
            _rotateTween?.Kill();   // only kill the rotation tween — leaves position alone
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

        // Kill any active tweens before Unity tears the transform down. Without this, the
        // infinite spin/shake loops keep running for one more frame after the GameObject is
        // destroyed (e.g. when DayCity unloads on sanity-0 → NightCity load) and DOTween
        // raises a MissingReferenceException on the dead RectTransform.
        private void OnDestroy()
        {
            _moveTween?.Kill();
            _rotateTween?.Kill();
        }
    }
}