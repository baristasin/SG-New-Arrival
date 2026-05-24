using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class AdmissionData
    {
        public string Name;
        public string AdmissionDeadlineDate;
    }

    public class AdmissionDocument : VisaDocumentBase<AdmissionData>
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _deadlineDateText;

        public override void Initialize(AdmissionData data)
        {
            base.Initialize(data);
            _nameText.text = data.Name;
            _deadlineDateText.SetText(data.AdmissionDeadlineDate);
        }
    }
}
