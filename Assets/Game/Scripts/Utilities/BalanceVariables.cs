using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Utilities
{
    [CreateAssetMenu(fileName = "BalanceVariables", menuName = "ScriptableObjects/BalanceVariables", order = 1)]
    public class BalanceVariables : ScriptableObject
    {
        [Serializable]
        public class MinigameSanityOverride
        {
            public MinigameId Minigame;
            public float DrainPerSecond = 1f;
        }
        #region Singleton
        private static BalanceVariables _instance;

        public static BalanceVariables Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Resources klasörü içinde bu isimle arama yapar
                    _instance = Resources.Load<BalanceVariables>("BalanceVariables");

                    if (_instance == null)
                    {
                        Debug.LogError("BalanceVariables ScriptableObject bulunamadı! " +
                                       "Lütfen 'Resources' klasörü içinde 'BalanceVariables' adıyla oluşturun.");
                    }
                }
                return _instance;
            }
        }
        #endregion
        
        public float ZombieLureDistance;
        public float ZombieAttackDistance;

        public float MaxSanity = 100f;
        public float SanityDrainPerSecond = 1f;
        public float LateDrainPerSecond = 0.5f;    // drain while late (past 10AM)

        public List<MinigameSanityOverride> MinigameDrainOverrides = new();

        public float GetMinigameDrain(MinigameId id)
        {
            if (MinigameDrainOverrides != null)
                for (int i = 0; i < MinigameDrainOverrides.Count; i++)
                    if (MinigameDrainOverrides[i] != null && MinigameDrainOverrides[i].Minigame == id)
                        return MinigameDrainOverrides[i].DrainPerSecond;
            return SanityDrainPerSecond;
        }

        public float StableThreshold = 75f;        // >= 75 : Stable
        public float UnsettledThreshold = 50f;     // >= 50 : Unsettled
        public float DisturbedThreshold = 25f;     // >= 25 : Disturbed, else Critical
    }
}