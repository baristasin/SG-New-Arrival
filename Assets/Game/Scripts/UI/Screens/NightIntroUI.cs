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

        public IEnumerator Play()
        {
            _dismissed = false;
            yield return Show().WaitForCompletion();
            while (!_dismissed) yield return null;
            yield return Hide().WaitForCompletion();
        }

        public void Dismiss() => _dismissed = true;
    }
}
