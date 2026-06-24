using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Screens
{
    // Always-on overlay while the city is active: clock label + sanity bar + mission interaction
    // prompts. Subscribes to the GameManager's day clock while shown.
    public class DayHUD : UIScreen
    {
        [SerializeField] private TMP_Text _timeText;

        public override Tween Show()
        {
            BindClock(true);
            return base.Show();
        }

        public override Tween Hide()
        {
            BindClock(false);
            return base.Hide();
        }

        private void BindClock(bool subscribe)
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.Clock == null) return;

            gm.Clock.OnTimeChanged -= UpdateTime;
            if (subscribe)
            {
                gm.Clock.OnTimeChanged += UpdateTime;
                UpdateTime(gm.Clock.Hour, gm.Clock.Minute);
            }
        }

        private void UpdateTime(int hour, int minute)
        {
            if (_timeText != null) _timeText.text = $"{hour:00}:{minute:00}";
        }
    }
}
