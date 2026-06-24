using UnityEngine;

namespace Game.Scripts.SanityModules
{
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
