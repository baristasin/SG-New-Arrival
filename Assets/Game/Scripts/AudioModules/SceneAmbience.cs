using UnityEngine;

namespace Game.Scripts.AudioModules
{
    [RequireComponent(typeof(AudioSource))]
    public class SceneAmbience : MonoBehaviour
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;
        [SerializeField, Min(0f)] private float _startDelay = 0f;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.clip = _clip;
            _source.loop = true;
            _source.playOnAwake = false;
            _source.volume = _volume;
        }

        private void OnEnable()
        {
            if (_clip == null) return;
            if (_startDelay > 0f) _source.PlayDelayed(_startDelay);
            else                  _source.Play();
        }

        private void OnDisable()
        {
            if (_source != null) _source.Stop();
        }
    }
}
