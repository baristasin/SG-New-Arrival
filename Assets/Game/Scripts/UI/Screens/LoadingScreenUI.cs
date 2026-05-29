using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Screens
{
    // Loading overlay shown across real scene loads and during the short in-scene minigame
    // transition. Subscribes to SceneLoader.OnProgress on Show and unsubscribes on Hide so the
    // bar stays in sync without listening when invisible.
    public class LoadingScreenUI : UIScreen
    {
        [Tooltip("Optional radial/horizontal fill image. Null = no bar, just the overlay.")]
        [SerializeField] private Image _progressFill;
        [SerializeField] private TMP_Text _progressText;

        public override Tween Show()
        {
            SetProgress(0f);
            if (SceneLoader.Instance != null) SceneLoader.Instance.OnProgress += SetProgress;
            return base.Show();
        }

        public override Tween Hide()
        {
            if (SceneLoader.Instance != null) SceneLoader.Instance.OnProgress -= SetProgress;
            return base.Hide();
        }

        public void SetProgress(float p)
        {
            p = Mathf.Clamp01(p);
            if (_progressFill != null) _progressFill.fillAmount = p;
            if (_progressText != null) _progressText.text = $"{Mathf.RoundToInt(p * 100f)}%";
        }
    }
}
