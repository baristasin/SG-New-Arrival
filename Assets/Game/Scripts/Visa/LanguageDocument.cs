using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Visa
{
    public enum LanguageLevel
    {
        A1, A2, B1, B2, C1
    }

    [Serializable]
    public class LanguageData
    {
        public LanguageLevel Level;
    }

    public class LanguageDocument : VisaDocumentBase<LanguageData>
    {
        [SerializeField] private TextMeshProUGUI _languageLevelText;

        public override void Initialize(LanguageData data)
        {
            base.Initialize(data);
            _languageLevelText.SetText(data.Level.ToString());
        }
    }
}
