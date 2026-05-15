using System;
using Game.Scripts.ApartmentHunting.Data;

namespace Game.Scripts.ApartmentHunting
{
    [Serializable]
    public class ApartmentPaperData
    {
        public string ApartmentName;
        public bool ProvidesAnmeldung;
        public BudgetCategory PriceCategory;
        public bool IsDormitory;

        public string AnmeldungStory;
        public string PriceStory;
        public string DormitoryStory;
    }
}