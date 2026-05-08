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
    }    
}