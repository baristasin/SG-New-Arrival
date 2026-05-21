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
    }
}
