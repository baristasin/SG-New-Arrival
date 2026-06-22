using System.Collections;
using DG.Tweening;

namespace Game.Scripts.UI.Screens
{
    // Fullscreen warning shown right before the zombie fight starts — explains the input
    // controls (movement, aim, fire). One image with a single fullscreen button wired to
    // NightIntroUI.Dismiss via Inspector OnClick. Play() returns when the button is clicked.
    public class NightIntroUI : UIScreen
    {
        private bool _dismissed;

        // onShown fires after fade-in — caller hides Loading cover here.
        // onDismissed fires after click but before fade-out — caller can raise cover for the
        // next transition if needed.
        public IEnumerator Play(System.Action onShown = null, System.Action onDismissed = null)
        {
            _dismissed = false;
            yield return Show().WaitForCompletion();
            onShown?.Invoke();
            while (!_dismissed) yield return null;
            onDismissed?.Invoke();
            yield return Hide().WaitForCompletion();
        }

        public void Dismiss() => _dismissed = true;
    }
}
