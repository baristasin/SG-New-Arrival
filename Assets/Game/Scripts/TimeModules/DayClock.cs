using System;

namespace Game.Scripts.TimeModules
{
    // In-game day clock. Plain (non-MonoBehaviour) so it can be owned by GameManager which drives
    // its Tick from Update. Counts in-game minutes against a real-second budget; pauses while the
    // player is inside a minigame and resumes when they return to the city.
    public class DayClock
    {
        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public bool IsRunning { get; private set; }

        // Real seconds per in-game minute. Default 1 = a 24h day takes 24 minutes (we never run
        // the full 24h, but the rate is what matters for the 8→10 AM mission window).
        public float SecondsPerInGameMinute { get; set; } = 1f;

        public event Action<int, int> OnTimeChanged;   // (hour, minute) — every minute tick
        public event Action<int> OnHourPassed;         // (newHour) — every time we cross :00

        private float _tickTimer;

        public void Reset(int startHour)
        {
            Hour = startHour;
            Minute = 0;
            _tickTimer = 0f;
            IsRunning = false;
            OnTimeChanged?.Invoke(Hour, Minute);
        }

        public void Resume() => IsRunning = true;
        public void Pause()  => IsRunning = false;

        // GameManager.Update calls this every frame with Time.deltaTime.
        public void Tick(float deltaTime)
        {
            if (!IsRunning) return;

            _tickTimer += deltaTime;
            while (_tickTimer >= SecondsPerInGameMinute)
            {
                _tickTimer -= SecondsPerInGameMinute;
                AdvanceMinute();
            }
        }

        private void AdvanceMinute()
        {
            Minute++;
            bool hourTicked = false;
            if (Minute >= 60)
            {
                Minute = 0;
                Hour++;
                hourTicked = true;
            }

            OnTimeChanged?.Invoke(Hour, Minute);
            if (hourTicked) OnHourPassed?.Invoke(Hour);
        }

        public string GetTimeString() => $"{Hour:00}:{Minute:00}";
    }
}
