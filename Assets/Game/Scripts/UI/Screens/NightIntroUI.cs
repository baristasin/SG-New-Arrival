using System.Collections;
using DG.Tweening;

namespace Game.Scripts.UI.Screens
{
    public class NightIntroUI : UIScreen
    {
        private bool _dismissed;
        
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
