using Game.Scripts.SanityModules;
using UnityEngine;

namespace Game.Scripts.Anmeldung
{
    // Sanity corruption for the Anmeldung minigame: as sanity drops, the current paper degrades.
    // For now each stage just logs; the AnmeldungDocument effect calls are commented out — pick the
    // ones you like (StartRotating / StartShaking / StartSpinning) and tune their values per stage.
    public class AnmeldungSanityCorruption : SanityCorruptionHandler
    {
        private AnmeldungDocument _paper;

        // Called by AnmeldungManager whenever a new paper is spawned, so the corruption follows it.
        public void SetPaper(AnmeldungDocument paper)
        {
            _paper = paper;
            ApplyStage(Sanity != null ? Sanity.Stage : SanityStage.Stable);
        }

        protected override void ApplyStage(SanityStage stage)
        {
            Debug.Log($"[AnmeldungCorruption] stage = {stage}");

            if (_paper == null) return;
            _paper.StopEffects();

            switch (stage)
            {
                case SanityStage.Stable:
                    // clean paper, no effect
                    break;

                case SanityStage.Unsettled:
                    Debug.Log("[AnmeldungCorruption] Unsettled — pick an effect");
                    // _paper.StartRotating(3f, 1.5f);
                    break;

                case SanityStage.Disturbed:
                    Debug.Log("[AnmeldungCorruption] Disturbed — pick an effect");
                    // _paper.StartRotating(8f, 1f);
                    // _paper.StartShaking(8f, 0.5f);
                    break;

                case SanityStage.Critical:
                    Debug.Log("[AnmeldungCorruption] Critical — pick an effect");
                    // _paper.StartSpinning(2f);
                    // _paper.StartShaking(15f, 0.4f);
                    break;
            }
        }
    }
}
