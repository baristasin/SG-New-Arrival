using System;
using Game.Scripts.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.SanityModules
{
    // Holds the player's sanity (0..MaxSanity), drains it over time, and raises events as the
    // value and stage change. Minigames listen to OnStageChanged to apply their corruptions.
    // Tuning lives in BalanceVariables. (Per-scene for now; becomes persistent with the city flow.)
    public class SanityManager : PersistentSingleton<SanityManager>
    {
        public float Sanity { get; private set; }
        public SanityStage Stage { get; private set; }

        // Set by the active context: the minigame rate while playing, a slower rate when late, 0 to pause.
        public float DrainPerSecond { get; set; }

        public float Normalized =>
            BalanceVariables.Instance.MaxSanity > 0f ? Sanity / BalanceVariables.Instance.MaxSanity : 0f;

        public event Action<float> OnSanityChanged;        // passes the normalized 0..1 value
        public event Action<SanityStage> OnStageChanged;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;   // a duplicate was destroyed by the base
            ResetSanity();
        }

        private void Update()
        {
            if (DrainPerSecond != 0f)
                Drain(DrainPerSecond * Time.deltaTime);
        }

        public void ResetSanity()
        {
            Sanity = BalanceVariables.Instance.MaxSanity;
            DrainPerSecond = BalanceVariables.Instance.SanityDrainPerSecond;
            Stage = ComputeStage(Sanity);
            OnSanityChanged?.Invoke(Normalized);
            OnStageChanged?.Invoke(Stage);
        }

        public void Drain(float amount) => SetSanity(Sanity - amount);

        public void SetSanity(float value)
        {
            value = Mathf.Clamp(value, 0f, BalanceVariables.Instance.MaxSanity);
            if (Mathf.Approximately(value, Sanity)) return;

            Sanity = value;
            OnSanityChanged?.Invoke(Normalized);

            var newStage = ComputeStage(Sanity);
            if (newStage != Stage)
            {
                Stage = newStage;
                OnStageChanged?.Invoke(Stage);
            }
        }

        private static SanityStage ComputeStage(float sanity)
        {
            var b = BalanceVariables.Instance;
            if (sanity >= b.StableThreshold) return SanityStage.Stable;
            if (sanity >= b.UnsettledThreshold) return SanityStage.Unsettled;
            if (sanity >= b.DisturbedThreshold) return SanityStage.Disturbed;
            return SanityStage.Critical;
        }

        [Button] private void DebugDrain10() => Drain(10f);
        [Button] private void DebugRefill() => ResetSanity();
    }
}
