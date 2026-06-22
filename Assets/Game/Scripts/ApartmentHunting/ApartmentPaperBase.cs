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

        public override void Initialize(ApartmentEntry data)
        {
            base.Initialize(data);
            _apartmentNameText.text = data.Name;
            // Price uses the int rent amount; Anmeldung + Schufa derive from the booleans; Type
            // stays straight from the enum.
            _priceStoryText.text = $"{data.RentAmount}€ per month";
            _anmeldungStoryText.text = data.ProvidesWohnungsgeberbescheinigung
                ? "Can be used for registration"
                : "It is only for short stays";
            _dormitoryStoryText.text = data.Type.ToString();
            _schufaStoryText.text = data.RequiresSchufa
                ? "You must provide Schufa credit report"
                : "There is no need for Schufa";

            if (_addressText != null)
                _addressText.text = data.Address;
            if (_landlordText != null)
                _landlordText.text = data.Landlord;
            if (_apartmentImage != null && ApartmentHuntingManager.Active != null)
                _apartmentImage.sprite = ApartmentHuntingManager.Active.GetApartmentSpriteForId(data.Id);
        }
    }
}
