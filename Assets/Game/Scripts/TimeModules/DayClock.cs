using System;

namespace Game.Scripts.TimeModules
{

    public class DayClock
    {
        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public bool IsRunning { get; private set; }


        public float SecondsPerInGameMinute { get; set; } = 1f;

        public event Action<int, int> OnTimeChanged;   
        public event Action<int> OnHourPassed;         

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
