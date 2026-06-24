using System;
using System.Collections.Generic;
using Game.Scripts.SanityModules;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Scripts.UI
{
    // Drives URP post-process overrides on the DayCity Global Volume based on the active
    // minigame and the player's current sanity. While sanity is BELOW _sanityThreshold, the
    // configured effect for the active minigame lerps from 0 up to its target intensity. When
    // sanity recovers or the minigame ends, intensities lerp back to 0.
    //
    // Lives in the DayCity scene next to the Global Volume.
    public class DaySanityVolumeEffects : MonoBehaviour
    {
        public enum EffectType { ChromaticAberration, Vignette, ColorAdjustments }

        [Serializable]
        public class MinigameEffect
        {
            public MinigameId Minigame;
            public EffectType Effect;
            [Range(-150f, 150f)] public float TargetIntensity = 0.5f;
        }

        [SerializeField] private VolumeProfile _profile;
        [SerializeField] private float _sanityThreshold = 10f;
        [SerializeField] private float _lerpSpeed = 1f;
        [SerializeField] private List<MinigameEffect> _config = new();

        private ChromaticAberration _chromatic;
        private Vignette _vignette;
        private ColorAdjustments _colorAdjustments;
        private float _baseChromatic, _baseVignette, _baseLens;

        private void Awake()
        {
            if (_profile == null) return;
            _profile.TryGet(out _chromatic);
            _profile.TryGet(out _vignette);
            _profile.TryGet(out _colorAdjustments);

            // Remember the inspector-authored starting values so we can blend out cleanly when
            // sanity is healthy / the player isn't inside a minigame.
            _baseChromatic = _chromatic != null ? _chromatic.intensity.value : 0f;
            _baseVignette  = _vignette  != null ? _vignette.intensity.value  : 0f;
            _baseLens      = _colorAdjustments      != null ? _colorAdjustments.hueShift.value      : 0f;
        }

        private void Update()
        {
            float chromaticTarget = _baseChromatic;
            float vignetteTarget  = _baseVignette;
            float lensTarget      = _baseLens;

            if (ShouldApplyEffect(out var activeId))
            {
                foreach (var c in _config)
                {
                    if (c.Minigame != activeId) continue;
                    switch (c.Effect)
                    {
                        case EffectType.ChromaticAberration: chromaticTarget = c.TargetIntensity; break;
                        case EffectType.Vignette:            vignetteTarget  = c.TargetIntensity; break;
                        case EffectType.ColorAdjustments:      lensTarget      = c.TargetIntensity; break;
                    }
                }
            }

            float t = _lerpSpeed * Time.deltaTime;
            if (_chromatic != null) _chromatic.intensity.value = Mathf.Lerp(_chromatic.intensity.value, chromaticTarget, t);
            if (_vignette  != null) _vignette.intensity.value  = Mathf.Lerp(_vignette.intensity.value,  vignetteTarget,  t);
            if (_colorAdjustments      != null) _colorAdjustments.hueShift.value      = Mathf.Lerp(_colorAdjustments.hueShift.value,      lensTarget,      t);
        }

        private bool ShouldApplyEffect(out MinigameId activeId)
        {
            activeId = default;
            var gm = GameManager.Instance;
            var sanity = SanityManager.Instance;
            if (gm == null || sanity == null) return false;
            if (gm.State != GameState.Minigame) return false;
            if (sanity.Sanity >= _sanityThreshold) return false;
            if (!gm.CurrentMinigameId.HasValue) return false;
            activeId = gm.CurrentMinigameId.Value;
            return true;
        }
    }
}
