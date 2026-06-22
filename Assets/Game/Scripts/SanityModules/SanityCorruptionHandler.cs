using UnityEngine;

namespace Game.Scripts.SanityModules
{
    // Base for a minigame's sanity corruption. Subscribes to the persistent SanityManager's stage
    // changes and applies the matching corruption. Works across scenes: the manager persists, this
    // handler lives in the minigame scene and (un)subscribes on enable/disable, applying the
    // current stage on enable (the event only fires on a change).
    public abstract class SanityCorruptionHandler : MonoBehaviour
    {
        protected SanityManager Sanity { get; private set; }

        protected virtual void OnEnable()
        {
            Sanity = SanityManager.Instance;
            Sanity.OnStageChanged += ApplyStage;
            ApplyStage(Sanity.Stage);
        }

        protected virtual void OnDisable()
        {
            if (Sanity != null)
                Sanity.OnStageChanged -= ApplyStage;
        }

        protected abstract void ApplyStage(SanityStage stage);
    }
}
