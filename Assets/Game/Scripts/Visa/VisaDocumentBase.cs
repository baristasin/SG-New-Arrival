using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.Visa
{
    public interface IClickableDocument
    {
        bool IsOpen { get; }
        bool IsAnimating { get; }
        bool IsActive { get; }
        float SlideDuration { get; }
        float SlideOutDuration { get; }
        void Open();
        void Close();
        void SetHighlight(bool on);
        void SlideIn(float delay);
        void SlideOut(float delay);
        void SnapHome();
    }

    public abstract class VisaDocumentBase<TData> : MonoBehaviour, IClickableDocument
    {
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private float _animDuration = 0.4f;

        [Header("Round entrance / exit slide")]
        [Tooltip("World-space Transform the doc slides from on entry and to on exit. Place an empty " +
                 "GameObject off the side of the desk and drag it here.")]
        [SerializeField] private Transform _slideAnchor;
        [SerializeField] private float _slideDuration = 0.5f;       // entrance speed
        [SerializeField] private float _slideOutDuration = 0.25f;   // exit speed (usually a bit faster)

        // Yellow hover highlight (e.g. an outline sprite child). Optional — toggled on hover.
        [SerializeField] private GameObject _highlight;

        public TData Data { get; private set; }
        public bool IsOpen => _isOpen;
        public bool IsAnimating => _isAnimating;
        public bool IsActive => gameObject.activeSelf;
        public float SlideDuration => _slideDuration;
        public float SlideOutDuration => _slideOutDuration;

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private bool _isOpen;
        private bool _isAnimating;

        protected virtual void Awake()
        {
            // Scene-placed documents capture their authored pose here. Spawned documents are
            // instantiated at their target pose too, so this still records the correct spot
            // (the spawn flow also re-captures it after the slide-in, which is harmless).
            SetOriginalPose();
            SetHighlight(false);
        }

        public void SetOriginalPose()
        {
            _originalPosition = transform.position;
            _originalRotation = transform.rotation;
        }

        public virtual void Initialize(TData data)
        {
            Data = data;
        }

        public void SetHighlight(bool on)
        {
            if (_highlight != null)
                _highlight.SetActive(on);
        }

        public void Open()
        {
            if (_isOpen) return;

            transform.DOKill();
            _isAnimating = true;
            _isOpen = true;
            SetHighlight(false);

            transform.DOMove(_cameraTarget.position, _animDuration).SetEase(Ease.OutQuad);
            transform.DORotateQuaternion(_cameraTarget.rotation, _animDuration).SetEase(Ease.OutQuad)
                .OnComplete(() => _isAnimating = false);
        }

        // Interruptible: can be called mid-open (e.g. via ESC) to reverse from wherever it is.
        public void Close()
        {
            if (!_isOpen) return;

            transform.DOKill();
            _isAnimating = true;
            _isOpen = false;

            transform.DOMove(_originalPosition, _animDuration).SetEase(Ease.OutQuad);
            transform.DORotateQuaternion(_originalRotation, _animDuration).SetEase(Ease.OutQuad)
                .OnComplete(() => _isAnimating = false);
        }

        // Snap to the slide anchor's world position, then glide to the original (placed) pose.
        public void SlideIn(float delay)
        {
            transform.DOKill();
            _isOpen = false;
            _isAnimating = true;
            SetHighlight(false);

            Vector3 startPos = _slideAnchor != null ? _slideAnchor.position : _originalPosition;
            transform.position = startPos;
            transform.rotation = _originalRotation;

            transform.DOMove(_originalPosition, _slideDuration).SetEase(Ease.OutQuad).SetDelay(delay)
                .OnComplete(() => _isAnimating = false);
        }

        // Glide off to the slide anchor's world position (straightens rotation on the way out).
        public void SlideOut(float delay)
        {
            transform.DOKill();
            _isOpen = false;
            _isAnimating = true;
            SetHighlight(false);

            Vector3 targetPos = _slideAnchor != null ? _slideAnchor.position : _originalPosition;

            transform.DOMove(targetPos, _slideOutDuration).SetEase(Ease.InQuad).SetDelay(delay)
                .OnComplete(() => _isAnimating = false);
            transform.DORotateQuaternion(_originalRotation, _slideOutDuration).SetEase(Ease.InQuad).SetDelay(delay);
        }

        // Snap to the placed (original) pose with no animation. Used on the very first appearance
        // so docs just "are there" instead of sliding in.
        public void SnapHome()
        {
            transform.DOKill();
            transform.position = _originalPosition;
            transform.rotation = _originalRotation;
            _isOpen = false;
            _isAnimating = false;
            SetHighlight(false);
        }
    }
}
