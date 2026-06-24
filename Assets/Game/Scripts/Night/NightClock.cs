using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Night
{
    // Scene-local night clock for the NightCity survival run. Ticks in-game minutes from
    // _startHour to _endHour at _realSecondsPerHour. Pauses on disable. Hour ticks and end-of-
    // night are exposed as events so NightCombatGate / NightUI can react. Defaults: 20→23
    // (8PM→11PM) at 60 real seconds per in-game hour = 3 minutes total.
    public class NightClock : MonoBehaviour
    {
        [SerializeField] private int _startHour = 20;
        [SerializeField] private int _endHour = 23;
        [SerializeField] private float _realSecondsPerHour = 60f;
        [SerializeField] private bool _autoStartOnEnable = false;

        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public bool IsRunning { get; private set; }
        public int StartHour => _startHour;
        public int EndHour => _endHour;

        public event Action<int, int> OnTimeChanged;   // (hour, minute) every in-game minute
        public event Action<int> OnHourPassed;          // (newHour) when we cross :00
        public event Action OnEndReached;               // we hit endHour:00 — night is over

        private float _secondsPerMinute;
        private float _tickTimer;
        private bool _endFired;

        private void Awake()
        {
            RecalcRate();
            ResetClock();
        }

        private void OnEnable()
        {
            if (_autoStartOnEnable) StartClock();
        }

        public void RecalcRate() => _secondsPerMinute = Mathf.Max(0.0001f, _realSecondsPerHour / 60f);

        [Button]
        public void ResetClock()
        {
            Hour = _startHour;
            Minute = 0;
            _tickTimer = 0f;
            _endFired = false;
            IsRunning = false;
            OnTimeChanged?.Invoke(Hour, Minute);
        }

        [Button] public void StartClock() => IsRunning = true;
        public void Pause() => IsRunning = false;
        public void Resume() => IsRunning = true;

        private void Update()
        {
            if (!IsRunning) return;
            if (_endFired) return;

            _tickTimer += Time.deltaTime;
            while (_tickTimer >= _secondsPerMinute)
            {
                _tickTimer -= _secondsPerMinute;
                AdvanceMinute();
                if (_endFired) return;
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

            if (Hour >= _endHour)
            {
                _endFired = true;
                IsRunning = false;
                OnEndReached?.Invoke();
            }
        }

        public string GetTimeString() => $"{Hour:00}:{Minute:00}";
    }
}
