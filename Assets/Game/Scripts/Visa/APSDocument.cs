using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class APSData
    {
        public bool APSRequired;
        public bool APSCompleted;
    }

    public class APSDocument : VisaDocumentBase<APSData>
    {
        [SerializeField] private TextMeshProUGUI _apsRequiredText;
        [SerializeField] private TextMeshProUGUI _apsCompletedText;

        public override void Initialize(APSData data)
        {
            base.Initialize(data);
            _apsRequiredText.SetText(data.APSRequired ? "Required" : "Not Required");
            _apsCompletedText.SetText(data.APSCompleted ? "Completed" : "Not Completed");
        }
    }
}
