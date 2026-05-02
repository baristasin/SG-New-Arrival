using UnityEngine;

namespace Game.Scripts.EconomyModules
{
    public class PlayerWallet : MonoBehaviour
    {
        public PlayerWalletData PlayerWalletData { get; private set; } // Will be save/load data
        public void Initialize(PlayerWalletData playerWalletData)
        {
            PlayerWalletData = playerWalletData;
        }

        public void AddMoneyAmount(int amount)
        {
            PlayerWalletData.MoneyAmount += amount;
        }

        public void RemoveMoneyAmount(int amount)
        {
            PlayerWalletData.MoneyAmount -= amount;
        }
    }
}