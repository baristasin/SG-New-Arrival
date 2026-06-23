using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    // Continuous zombie-growl ambience. Spawns N concurrent 2D loop instances — atmospheric
    // crowd murmur, no spatial attenuation, so it's audible wherever the player stands. Each
    // instance's volume scales with how many zombies are actually alive (ZombieRegistry.Count)
    // so the soundscape is loud when the streets are packed and thin when they're cleared.
    // Lives in NightCity.
    public class ZombieAmbience : MonoBehaviour
    {
        [SerializeField] private EventReference _growlLoopEvent;
        [Tooltip("How many concurrent crowd voices play at once. Multiple stacks for a layered " +
                 "murmur; one is enough if the event already has internal variation.")]
        [SerializeField, Min(1)] private int _voiceCount = 3;

        [Header("Volume vs. crowd size")]
        [Tooltip("Volume when no zombies are alive — kept low for a faint distant murmur.")]
        [SerializeField, Range(0f, 2f)] private float _minVolume = 0.15f;
        [Tooltip("Volume at FullCrowdAt or more zombies.")]
        [SerializeField, Range(0f, 2f)] private float _maxVolume = 1f;
        [Tooltip("Live zombie count that locks the ambience to MaxVolume.")]
        [SerializeField, Min(1)] private int _fullCrowdAt = 30;
        [Tooltip("How often (seconds) we update the volume — no need to do it every frame.")]
        [SerializeField] private float _volumeUpdateInterval = 0.5f;

        private readonly List<EventInstance> _voices = new();
        private float _nextVolumeUpdate;

        private void OnEnable()
        {
            if (_growlLoopEvent.IsNull) return;

            for (int i = 0; i < _voiceCount; i++)
            {
                // 2D loop — atmosphere, not a worldspace source. Audible at full volume across
                // the whole map until the crowd is thinned out.
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
