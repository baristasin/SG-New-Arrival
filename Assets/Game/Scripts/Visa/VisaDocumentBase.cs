using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.Visa
{
    public interface IClickableDocument
    {
        void OnClick();
    }

    public abstract class VisaDocumentBase<TData> : MonoBehaviour, IClickableDocument
    {
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private float _animDuration = 0.4f;

        public TData Data { get; private set; }

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private bool _isOpen;
        private bool _isAnimating;

        public void SetOriginalPose()
        {
            _originalPosition = transform.position;
            _originalRotation = transform.rotation;
        }

        public virtual void Initialize(TData data)
        {
            Data = data;
        }

        public void OnClick()
        {
            if (_isAnimating) return;

            if (_isOpen)
                Close();
            else
                Open();
        }

        private void Open()
        {
            _isAnimating = true;
            _isOpen = true;

            transform.DOMove(_cameraTarget.position, _animDuration).SetEase(Ease.OutQuad);
            transform.DORotateQuaternion(_cameraTarget.rotation, _animDuration).SetEase(Ease.OutQuad)
                .OnComplete(() => _isAnimating = false);
        }

        private void Close()
        {
            _isAnimating = true;
            _isOpen = false;

            transform.DOMove(_originalPosition, _animDuration).SetEase(Ease.OutQuad);
            transform.DORotateQuaternion(_originalRotation, _animDuration).SetEase(Ease.OutQuad)
                .OnComplete(() => _isAnimating = false);
        }
    }
}
