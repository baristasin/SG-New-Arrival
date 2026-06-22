using UnityEngine;

namespace Game.Scripts.Utilities
{
    [CreateAssetMenu(fileName = "BalanceVariables", menuName = "ScriptableObjects/BalanceVariables", order = 1)]
    public class BalanceVariables : ScriptableObject
    {
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

        [Header("Sanity")]
        public float MaxSanity = 100f;
        public float SanityDrainPerSecond = 1f;   // drain while in a minigame (the "1-by-1" drop)
        public float LateDrainPerSecond = 0.5f;    // drain while late (past 10AM) — used by the flow later

        [Header("Sanity stage thresholds (sanity >= ...)")]
        public float StableThreshold = 75f;        // >= 75 : Stable
        public float UnsettledThreshold = 50f;     // >= 50 : Unsettled
        public float DisturbedThreshold = 25f;     // >= 25 : Disturbed, else Critical
    }
}