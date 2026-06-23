using FMOD.Studio;
using FMODUnity;
using Game.Scripts.Utilities;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Game.Scripts.AudioModules
{
    // Persistent music conductor. One looping track at a time, with idempotent transitions:
    // PlayDay / PlayNight are safe to call repeatedly (they no-op if the right track is already
    // running). Lives in the Bootstrap scene next to the other persistent singletons.
    public class MusicController : PersistentSingleton<MusicController>
    {
        public enum Track { None, Day, Night }

        [SerializeField] private EventReference _dayMusic;
        [SerializeField] private EventReference _nightMusic;
        [Tooltip("Volume multiplier. 1 = unity, values above 1 boost beyond the event's authored " +
                 "level (push too far and FMOD will clip). 0.5 ≈ -6dB, 2.0 ≈ +6dB.")]
        [SerializeField, Range(0f, 3f)] private float _dayVolume   = 1f;
        [SerializeField, Range(0f, 3f)] private float _nightVolume = 1f;

        private EventInstance _current;
        public Track ActiveTrack { get; private set; } = Track.None;

        public void PlayDay()   => Play(_dayMusic,   Track.Day,   _dayVolume);
        public void PlayNight() => Play(_nightMusic, Track.Night, _nightVolume);

        public void StopAll()
        {
            if (AudioManager.Instance == null) return;
            AudioManager.Instance.Stop(ref _current);
            ActiveTrack = Track.None;
        }

        private void Play(EventReference ev, Track track, float volume)
        {
            if (ActiveTrack == track && _current.isValid()) return;
            if (AudioManager.Instance == null) return;

            AudioManager.Instance.Stop(ref _current);
            _current = AudioManager.Instance.PlayLoop(ev);
            AudioManager.Instance.SetVolume(_current, volume);
            ActiveTrack = track;
        }

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
        }

        private void OnDestroy()
        {
            if (_current.isValid())
                _current.stop(STOP_MODE.IMMEDIATE);
        }
    }
}
