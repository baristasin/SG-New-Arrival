using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI
{
    // Base for every fullscreen UI panel under UIManager (MainMenu, Loading, DayStart, DayHUD,
    // Tutorial, EyeClose, DayRewards). Each owns a CanvasGroup; Show/Hide fades it in/out and
    // toggles raycast blocking. Screens stay GameObject-active and just become alpha 0 + non-
    // interactive when hidden — Awake clamps them to the hidden state so they can be wired in
    // the editor without flashing on load.
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreen : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected float _fadeDuration = 0.25f;

        public bool IsShown { get; private set; }

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
