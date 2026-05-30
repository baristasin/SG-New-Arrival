using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Screens
{
    // Loading overlay shown across real scene loads and the short in-scene minigame transition.
    // Lives at the BOTTOM of the UIManager canvas (renders behind every other screen) and uses
    // INSTANT show/hide — no fade. The screen sitting in front of it (MainMenu, DayStart, etc.)
    // does the visible fading; Loading just acts as a permanent opaque backdrop while it's up
    // so the underlying scene is never briefly visible between fades.
    public class LoadingScreenUI : UIScreen
    {
        [Tooltip("Optional radial/horizontal fill image. Null = no bar, just the overlay.")]
        [SerializeField] private Image _progressFill;
        [SerializeField] private TMP_Text _progressText;

        public override Tween Show()
        {
            if (IsShown) return null;
            IsShown = true;

            _canvasGroup.DOKill();
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;

            SetProgress(0f);
            if (SceneLoader.Instance != null) SceneLoader.Instance.OnProgress += SetProgress;
            return null;
        }

        public override Tween Hide()
        {
            if (!IsShown) return null;
            IsShown = false;

            _canvasGroup.DOKill();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            if (SceneLoader.Instance != null) SceneLoader.Instance.OnProgress -= SetProgress;
            return null;
        }

        public void SetProgress(float p)
        {
            p = Mathf.Clamp01(p);
            if (_progressFill != null) _progressFill.fillAmount = p;
            if (_progressText != null) _progressText.text = $"{Mathf.RoundToInt(p * 100f)}%";
        }
    }
}
