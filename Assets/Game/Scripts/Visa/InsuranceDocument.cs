using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class InsuranceData
    {
        public string IssuedPerson;
        public string DateOfBirth;
    }

    public class InsuranceDocument : VisaDocumentBase<InsuranceData>
    {
        [SerializeField] private TextMeshProUGUI _issuedPersonText;
        [SerializeField] private TextMeshProUGUI _dateOfBirthText;

        public override void Initialize(InsuranceData data)
        {
            base.Initialize(data);
            _issuedPersonText.SetText(data.IssuedPerson);
            _dateOfBirthText.SetText(data.DateOfBirth);
        }
    }
}
