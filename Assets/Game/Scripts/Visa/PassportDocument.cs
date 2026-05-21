using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class PassportData
    {
        public string Nationality;
        public int Age;
        public string PassportExpireDate;
    }

    public class PassportDocument : VisaDocumentBase<PassportData>
    {
        [SerializeField] private TextMeshProUGUI _nationalityText;
        [SerializeField] private TextMeshProUGUI _ageText;
        [SerializeField] private TextMeshProUGUI _expireDateText;

        public override void Initialize(PassportData data)
        {
            base.Initialize(data);
            _nationalityText.SetText(data.Nationality);
            _ageText.SetText("{0}", data.Age);
            _expireDateText.SetText(data.PassportExpireDate);
        }
    }
}
