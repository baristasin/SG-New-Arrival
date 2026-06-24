using Game.Scripts.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.AudioModules
{

    [RequireComponent(typeof(AudioSource))]
    public class UIClickSounds : PersistentSingleton<UIClickSounds>
    {
        [SerializeField] private AudioClip _clickClip;
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;

        private AudioSource _source;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            _source = GetComponent<AudioSource>();

            _source.playOnAwake = false;
            _source.loop = false;
            _source.spatialBlend = 0f;
            _source.mute = false;
            _source.volume = 1f;
            _source.outputAudioMixerGroup = null;
        }

        public void PlayClick()
        {
            if (_clickClip == null) { Debug.LogWarning("[UIClickSounds] _clickClip is null"); return; }
            if (_source == null)    { Debug.LogWarning("[UIClickSounds] _source is null");    return; }
            _source.PlayOneShot(_clickClip, _volume);
        }

        [Button("Play Click (Debug)")]
        private void DebugPlay() => PlayClick();
    }
}
