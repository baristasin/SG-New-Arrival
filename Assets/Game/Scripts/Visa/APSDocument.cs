using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class APSData
    {
        public string Name;
    }

    public class APSDocument : VisaDocumentBase<APSData>
    {
        [SerializeField] private TextMeshProUGUI _nameText;

        public override void Initialize(APSData data)
        {
            base.Initialize(data);
            _nameText.text = data.Name;
        }
    }
}
