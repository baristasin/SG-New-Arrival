using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class FundsData
    {
        public float BlockedAccountAmount;
    }

    public class FundsDocument : VisaDocumentBase<FundsData>
    {
        [SerializeField] private TextMeshProUGUI _blockedAccountText;

        public override void Initialize(FundsData data)
        {
            base.Initialize(data);
            _blockedAccountText.SetText("{0} EUR", (int)data.BlockedAccountAmount);
        }
    }
}
