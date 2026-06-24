using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public class ZombieAmbience : MonoBehaviour
    {
        [SerializeField] private EventReference _growlLoopEvent;
        [SerializeField, Min(1)] private int _voiceCount = 3;

        [SerializeField, Range(0f, 2f)] private float _minVolume = 0.15f;
        [SerializeField, Range(0f, 2f)] private float _maxVolume = 1f;
        [SerializeField, Min(1)] private int _fullCrowdAt = 30;
        [SerializeField] private float _volumeUpdateInterval = 0.5f;

        private readonly List<EventInstance> _voices = new();
        private float _nextVolumeUpdate;

        private void OnEnable()
        {
            if (_growlLoopEvent.IsNull) return;

            for (int i = 0; i < _voiceCount; i++)
            {

                var inst = AudioManager.Instance.PlayLoop(_growlLoopEvent);
                _voices.Add(inst);
            }
            ApplyVolumeNow();
        }

        private void OnDisable()
        {
            for (int i = 0; i < _voices.Count; i++)
            {
                var v = _voices[i];
                if (v.isValid()) AudioManager.Instance.Stop(ref v);
            }
            _voices.Clear();
        }

        private void Update()
        {
            if (Time.time < _nextVolumeUpdate) return;
            _nextVolumeUpdate = Time.time + _volumeUpdateInterval;
            ApplyVolumeNow();
        }

        private void ApplyVolumeNow()
        {
            float t = Mathf.Clamp01((float)ZombieRegistry.Count / _fullCrowdAt);
            float volume = Mathf.Lerp(_minVolume, _maxVolume, t);
            foreach (var v in _voices)
                if (v.isValid()) v.setVolume(volume);
        }
    }
}
