using System.Collections;
using Game.Scripts.SanityModules;
using UnityEngine;

namespace Game.Scripts.Anmeldung
{
    // Sanity corruption for the Anmeldung minigame:
    //  - Stable:     5 necessary items only, no paper effects
    //  - Unsettled:  + 3 distractor items (the wrong info)
    //  - Disturbed:  + paper starts spinning
    //  - Critical:   + a periodic tremble while it spins
    // AnmeldungManager queries ShouldShowDistractors when building each round.
    public class AnmeldungSanityCorruption : SanityCorruptionHandler
    {
        [Header("Paper spin (Disturbed and Critical)")]
        [SerializeField] private float _spinDuration = 40f;   // seconds per full rotation (higher = slower)

        [Header("Tremble (Critical only — periodic short shakes during spin)")]
        [SerializeField] private Vector2 _trembleIntervalRange = new Vector2(1.5f, 3f);
        [SerializeField] private float _trembleIntensity = 4f;
        [SerializeField] private float _trembleDuration = 0.3f;

        private AnmeldungDocument _paper;
        private Coroutine _trembleRoutine;

        public bool ShouldShowDistractors =>
            Sanity != null && Sanity.Stage > SanityStage.Stable;

        // Called by AnmeldungManager whenever a new paper is spawned, so the corruption follows it.
        public void SetPaper(AnmeldungDocument paper)
        {
            _paper = paper;
            ApplyStage(Sanity != null ? Sanity.Stage : SanityStage.Stable);
        }

        protected override void ApplyStage(SanityStage stage)
        {
            Debug.Log($"[AnmeldungCorruption] stage = {stage}");

            StopTremble();
            if (_paper == null) return;
            _paper.StopEffects();

            switch (stage)
            {
                case SanityStage.Stable:
                case SanityStage.Unsettled:
                    // no paper effects (item-count change is driven by ShouldShowDistractors)
                    break;

                case SanityStage.Disturbed:
                    _paper.StartSpinning(_spinDuration);
                    break;

                case SanityStage.Critical:
                    _paper.StartSpinning(_spinDuration);
                    StartTremble();
                    break;
            }
        }

        private void StartTremble()
        {
            if (_trembleRoutine != null) StopCoroutine(_trembleRoutine);
            _trembleRoutine = StartCoroutine(TrembleLoop());
        }

        private void StopTremble()
        {
            if (_trembleRoutine != null)
            {
                StopCoroutine(_trembleRoutine);
                _trembleRoutine = null;
            }
        }

        private IEnumerator TrembleLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_trembleIntervalRange.x, _trembleIntervalRange.y));
                if (_paper != null) _paper.ShakeOnce(_trembleIntensity, _trembleDuration);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopTremble();
        }
    }
}
