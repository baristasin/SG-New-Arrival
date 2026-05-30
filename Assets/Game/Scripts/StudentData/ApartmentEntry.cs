using System;

namespace Game.Scripts.StudentData
{
    [Serializable]
    public class ApartmentEntry
    {
        public int Id;
        public string Name;
        public string Address;
        public string Landlord;
        public EnglishProficiency LandlordEnglishProficiency;
        public bool ProvidesWohnungsgeberbescheinigung;
        public bool RequiresSchufa;
        public PriceCategory PriceCategory;
        public string PriceNotes;
        public ApartmentType Type;

        // Monthly rent in € — formatted on the paper as "{RentAmount} per month".
        public int RentAmount;
    }
}
