using System;
using Game.Scripts.StudentData;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class LanguageData
    {
        public string Provider;
        public string Name;
        public LanguageLevel Level;
        public string Date;
    }

    public class LanguageDocument : VisaDocumentBase<LanguageData>
    {
        [SerializeField] private TextMeshProUGUI _providerText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _languageLevelText;
        [SerializeField] private TextMeshProUGUI _date;

        public override void Initialize(LanguageData data)
        {
            base.Initialize(data);
            _providerText.text = data.Provider;
            _nameText.text = data.Name;
            _languageLevelText.SetText(data.Level.ToString());
            _date.text = data.Date;
        }
    }
}
