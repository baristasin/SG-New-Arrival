using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Scripts.UI
{
    // Drives a URP Lens Distortion override as a continuous sine wave for a "drunk / sanity
    // bad" screen feel. Read by default from any sanity source you wire up; or just leave the
    // amplitude on a fixed value for a constant effect.
    public class ScreenWaveEffect : MonoBehaviour
    {
        [SerializeField] private VolumeProfile _profile;
        [SerializeField] private float _frequency = 1.5f;
        [SerializeField, Range(0f, 1f)] private float _amplitude = 0.25f;
        [SerializeField, Range(-1f, 1f)] private float _baseIntensity = 0f;
        [Tooltip("Multiplies the amplitude. Drive this from outside (e.g. sanity normalized) " +
                 "to fade the effect in and out.")]
        [SerializeField, Range(0f, 1f)] private float _strength = 1f;

        private LensDistortion _lens;

        private void Awake()
        {
            if (_profile != null) _profile.TryGet(out _lens);
        }

        private void Update()
        {
            if (_lens == null) return;
            float wave = Mathf.Sin(Time.time * _frequency * Mathf.PI * 2f) * _amplitude * _strength;
            _lens.intensity.value = _baseIntensity + wave;
        }

        public void SetStrength(float s) => _strength = Mathf.Clamp01(s);
    }
}
