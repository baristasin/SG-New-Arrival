using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreen : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected float _fadeDuration = 0.25f;

        public bool IsShown { get; protected set; }

        protected virtual void Awake()
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            IsShown = false;
        }

        public virtual Tween Show()
        {
            if (IsShown) return null;
            IsShown = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            _canvasGroup.DOKill();
            return _canvasGroup.DOFade(1f, _fadeDuration).SetEase(Ease.OutQuad);
        }

        public virtual Tween Hide()
        {
            if (!IsShown) return null;
            IsShown = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            _canvasGroup.DOKill();
            return _canvasGroup.DOFade(0f, _fadeDuration).SetEase(Ease.OutQuad);
        }
    }
}
