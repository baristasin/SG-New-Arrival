using System;
using Game.Scripts.SaveModules;

namespace Game.Scripts.EconomyModules
{
    public class PlayerWalletData : SaveableData
    {
        private int _moneyAmount;

        public int MoneyAmount
        {
            get => _moneyAmount;
            set => SetField(ref _moneyAmount, value);
        }
    }
}