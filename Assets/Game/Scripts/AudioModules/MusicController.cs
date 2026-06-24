using FMOD.Studio;
using FMODUnity;
using Game.Scripts.Utilities;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Game.Scripts.AudioModules
{
    public class MusicController : PersistentSingleton<MusicController>
    {
        public enum Track { None, Day, Night }

        [SerializeField] private EventReference _dayMusic;
        [SerializeField] private EventReference _nightMusic;
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


        private void Update()
        {
            if (ActiveTrack == Track.None) return;
            if (!_current.isValid()) return;

            _current.getPlaybackState(out PLAYBACK_STATE state);
            if (state != PLAYBACK_STATE.STOPPED) return;

            var resume = ActiveTrack;
            ActiveTrack = Track.None;
            if (resume == Track.Day)        Play(_dayMusic,   Track.Day,   _dayVolume);
            else if (resume == Track.Night) Play(_nightMusic, Track.Night, _nightVolume);
        }

        private void OnDestroy()
        {
            if (_current.isValid())
                _current.stop(STOP_MODE.IMMEDIATE);
        }
    }
}
