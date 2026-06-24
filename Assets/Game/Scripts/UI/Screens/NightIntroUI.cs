using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Screens
{
    public class NightIntroUI : UIScreen
    {
        [SerializeField] private float _dismissBlockDuration = 3f;

        private bool _dismissed;
        private bool _canDismiss;

        public IEnumerator Play(System.Action onShown = null, System.Action onDismissed = null)
        {
            _dismissed = false;
            _canDismiss = false;

            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var b in buttons) if (b != null) b.interactable = false;

            yield return Show().WaitForCompletion();
            onShown?.Invoke();

            yield return new WaitForSeconds(_dismissBlockDuration);
            _canDismiss = true;
            foreach (var b in buttons) if (b != null) b.interactable = true;

            while (!_dismissed) yield return null;
            onDismissed?.Invoke();
            yield return Hide().WaitForCompletion();
        }

        public void Dismiss()
        {
            if (!_canDismiss) return;
            _dismissed = true;
        }
    }
}
