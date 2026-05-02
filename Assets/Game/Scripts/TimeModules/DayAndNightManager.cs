using System;
using UnityEngine;

namespace Game.Scripts.TimeModules
{
    public class DayAndNightManager : MonoBehaviour
    {
        public event Action<int, int> OnTimeChanged; // (hour, minute)
        public event Action OnDayEnded;

        [SerializeField] private int _startHour = 6;   // 6 AM
        [SerializeField] private int _endHour = 23;    // 11 PM

        [SerializeField] private float _secondsPerMinute = 1f;

        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }

        private float _tickTimer;

        public void Initialize()
        {
            Hour = _startHour;
            Minute = 0;
            _tickTimer = 0f;
            IsRunning = true;
            OnTimeChanged?.Invoke(Hour, Minute);
        }

        public void Stop() => IsRunning = false;
        public void Resume() => IsRunning = true;

        public void SetTickSpeed(float secondsPerMinute) => _secondsPerMinute = secondsPerMinute;

        private void Update()
        {
            if (!IsRunning) return;
            if (!IsPaused) return;

            _tickTimer += Time.deltaTime;
            if (_tickTimer < _secondsPerMinute) return;

            _tickTimer -= _secondsPerMinute;
            AdvanceMinute();
        }

        public void AdvanceTime(int minutes)
        {
            if (minutes <= 0 || !IsRunning) return;

            for (int i = 0; i < minutes; i++)
            {
                AdvanceMinute();
                if (!IsRunning) break; // day ended mid-skip
            }
        }

        private void AdvanceMinute()
        {
            Minute++;
            if (Minute >= 60)
            {
                Minute = 0;
                Hour++;
            }

            OnTimeChanged?.Invoke(Hour, Minute);

            if (Hour >= _endHour)
            {
                IsRunning = false;
                OnDayEnded?.Invoke();
            }
        }


        /// <summary> Format like "06:03" or "11:45". </summary>
        public string GetTimeString() => $"{Hour:00}:{Minute:00}";
    }
}