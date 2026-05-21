using Game.Scripts.StudentData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.ApartmentHunting
{
    public class ApartmentPaperBase : HuntingPaperBase<ApartmentEntry>
    {
        [SerializeField] private TextMeshProUGUI _apartmentNameText;

        [SerializeField] private TextMeshProUGUI _priceStoryText;

        [SerializeField] private TextMeshProUGUI _anmeldungStoryText;

        [SerializeField] private TextMeshProUGUI _dormitoryStoryText;

        [SerializeField] private TextMeshProUGUI _schufaStoryText;
        
        [SerializeField] private TextMeshProUGUI _addressText;

        [SerializeField] private TextMeshProUGUI _landlordText;
        
        [SerializeField] private Image _apartmentImage;

        [SerializeField] private Sprite[] _apartmentSprites;

        public override void Initialize(ApartmentEntry data)
        {
            base.Initialize(data);
            _apartmentNameText.text = data.Name;
            _priceStoryText.text = data.PriceCategory.ToString();
            _anmeldungStoryText.text = data.ProvidesWohnungsgeberbescheinigung ? "Yes anmeldung" : "No anmeldung";
            _dormitoryStoryText.text = data.Type.ToString();
            _schufaStoryText.text = data.RequiresSchufa ? "Yes schufa" : "No schufa";
            
            if (_addressText != null)
                _addressText.text = data.Address;
            if (_landlordText != null)
                _landlordText.text = data.Landlord;
            if (_apartmentImage != null)
                _apartmentImage.sprite = GetSpriteForId(data.Id);
        }

        private Sprite GetSpriteForId(int id)
        {
            int index = id - 1;
            if (_apartmentSprites != null && index >= 0 && index < _apartmentSprites.Length)
                return _apartmentSprites[index];

            Debug.LogWarning($"[ApartmentPaperBase] No sprite for apartment Id {id} " +
                             $"(assigned {_apartmentSprites?.Length ?? 0} sprites).");
            return null;
        }
    }
}
