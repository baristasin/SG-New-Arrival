using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class InsuranceData
    {
        public bool HasTravelInsurance;
        public bool HasHealthInsurance;
    }

    public class InsuranceDocument : VisaDocumentBase<InsuranceData>
    {
        [SerializeField] private TextMeshProUGUI _travelInsuranceText;
        [SerializeField] private TextMeshProUGUI _healthInsuranceText;

        public override void Initialize(InsuranceData data)
        {
            base.Initialize(data);
            _travelInsuranceText.SetText(data.HasTravelInsurance ? "Active" : "None");
            _healthInsuranceText.SetText(data.HasHealthInsurance ? "Active" : "None");
        }
    }
}
