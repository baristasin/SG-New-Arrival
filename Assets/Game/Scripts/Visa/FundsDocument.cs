using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class FundsData
    {
        public string FirstName;
        public string LastName;
        public string DateOfBirth;
        public string PassportNumber;
        public string IbanNumber;
        public float BlockedAccountAmount;
        
    }

    public class FundsDocument : VisaDocumentBase<FundsData>
    {
        [SerializeField] private TextMeshProUGUI _firstNameText;
        [SerializeField] private TextMeshProUGUI _lastNameText;
        [SerializeField] private TextMeshProUGUI _dateOfBirthText;
        [SerializeField] private TextMeshProUGUI _passportNumberText;
        [SerializeField] private TextMeshProUGUI _ibanNumberText;
        [SerializeField] private TextMeshProUGUI _blockedAccountAmountText;
        [SerializeField] private TextMeshProUGUI _monthlyAmountText;
        
        public override void Initialize(FundsData data)
        {
            base.Initialize(data);
            _firstNameText.text = data.FirstName;
            _lastNameText.text = data.LastName;
            _dateOfBirthText.text = data.DateOfBirth;
            _passportNumberText.text = data.PassportNumber;
            _ibanNumberText.text = data.IbanNumber;
            _blockedAccountAmountText.text = data.BlockedAccountAmount.ToString();
            var monthlyAmount = data.BlockedAccountAmount / 12f;
            _monthlyAmountText.text = monthlyAmount.ToString("0");
        }
    }
}
