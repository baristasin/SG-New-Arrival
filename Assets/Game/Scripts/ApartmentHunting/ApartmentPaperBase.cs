using TMPro;
using UnityEngine;

namespace Game.Scripts.ApartmentHunting
{
    public class ApartmentPaperBase : HuntingPaperBase<ApartmentPaperData>
    {
        [SerializeField] private TextMeshProUGUI _apartmentNameText;

        [SerializeField] private TextMeshProUGUI _priceStoryText;
        
        [SerializeField] private TextMeshProUGUI _anmeldungStoryText;
        
        [SerializeField] private TextMeshProUGUI _dormitoryStoryText;
        
        public override void Initialize(ApartmentPaperData data)
        {
            base.Initialize(data);
            _apartmentNameText.text = data.ApartmentName;
            _priceStoryText.text = data.PriceStory;
            _anmeldungStoryText.text = data.AnmeldungStory;
            _dormitoryStoryText.text = data.DormitoryStory;
        }
    }
}