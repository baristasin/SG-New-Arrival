using Game.Scripts.StudentData;
using TMPro;
using UnityEngine;

namespace Game.Scripts.ApartmentHunting
{
    public class ApartmentPaperBase : HuntingPaperBase<ApartmentEntry>
    {
        [SerializeField] private TextMeshProUGUI _apartmentNameText;

        [SerializeField] private TextMeshProUGUI _priceStoryText;

        [SerializeField] private TextMeshProUGUI _anmeldungStoryText;

        [SerializeField] private TextMeshProUGUI _dormitoryStoryText;

        public override void Initialize(ApartmentEntry data)
        {
            base.Initialize(data);
            _apartmentNameText.text = data.Name;
            _priceStoryText.text = data.PriceCategory.ToString();
            _anmeldungStoryText.text = data.ProvidesWohnungsgeberbescheinigung ? "Yes" : "No";
            _dormitoryStoryText.text = data.Type.ToString();
        }
    }
}
