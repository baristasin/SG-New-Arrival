using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class VisaApplyData
    {
        public bool FeePaid;
    }

    public class VisaApplyDocument : VisaDocumentBase<VisaApplyData>
    {
        [SerializeField] private TextMeshProUGUI _feePaidText;

        public override void Initialize(VisaApplyData data)
        {
            base.Initialize(data);
            _feePaidText.SetText(data.FeePaid ? "Paid" : "Not Paid");
        }
    }
}
