using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Night
{
    // Quick lightning flash on a Light component. Strike() ramps the intensity
    // peak → mid → peak → mid → base, then sits at base again. Auto-strike fires it on a
    // random interval so the night sky pulses without manual triggers.
    public class LightningEffect : MonoBehaviour
    {
        [SerializeField] private Light _light;
        [SerializeField] private float _baseIntensity = 4.3f;
        [SerializeField] private float _peakIntensity = 15f;
        [SerializeField] private float _midIntensity  = 10f;
        [SerializeField] private float _stepDuration = 0.06f;

        [SerializeField] private bool _autoStrike = true;
        [SerializeField] private Vector2 _autoIntervalRange = new Vector2(4f, 12f);

        private Coroutine _strikeRoutine;

        private void OnEnable()
        {
            if (_light != null) _light.intensity = _baseIntensity;
            if (_autoStrike) StartCoroutine(AutoStrikeLoop());
        }

        [Button]
        public void Strike()
        {
            if (_light == null) return;
            if (_strikeRoutine != null) StopCoroutine(_strikeRoutine);
            _strikeRoutine = StartCoroutine(StrikeRoutine());
        }

        private IEnumerator StrikeRoutine()
        {
            _light.intensity = _peakIntensity;
            yield return new WaitForSeconds(_stepDuration);
            _light.intensity = _midIntensity;
            yield return new WaitForSeconds(_stepDuration);
            _light.intensity = _peakIntensity;
            yield return new WaitForSeconds(_stepDuration);
            _light.intensity = _midIntensity;
            yield return new WaitForSeconds(_stepDuration);
            _light.intensity = _peakIntensity;
            yield return new WaitForSeconds(_stepDuration);
            _light.intensity = _midIntensity;
            yield return new WaitForSeconds(_stepDuration);
            _light.intensity = _peakIntensity;
            yield return new WaitForSeconds(_stepDuration);
            _light.intensity = _midIntensity;
            yield return new WaitForSeconds(_stepDuration);
            _light.intensity = _baseIntensity;
            _strikeRoutine = null;
        }

        private IEnumerator AutoStrikeLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_autoIntervalRange.x, _autoIntervalRange.y));
                Strike();
            }
        }
    }
}
