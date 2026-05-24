using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class PassportData
    {
        public Sprite Photo;
        public string Id;
        public string Name;
        public string Nationality;
        public string ExpiresAt;
        public string IssuedAt;
    }

    public class PassportDocument : VisaDocumentBase<PassportData>
    {
        [SerializeField] private Image _photoImage;
        [SerializeField] private TextMeshProUGUI _idText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _nationalityText;
        [SerializeField] private TextMeshProUGUI _expiresAtText;
        [SerializeField] private TextMeshProUGUI _issuedAtText;

        public override void Initialize(PassportData data)
        {
            base.Initialize(data);
            _photoImage.sprite = data.Photo;
            _idText.text = data.Id;
            _nameText.text = data.Name;
            _nationalityText.text = data.Nationality;
            _expiresAtText.text = data.ExpiresAt;
            _issuedAtText.text = data.IssuedAt;
        }
    }
}
