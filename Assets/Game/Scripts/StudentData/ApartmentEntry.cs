using System;
using UnityEngine;

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

        // Free-form flavour text shown on the apartment paper. Type still comes from the
        // ApartmentType enum, so no separate text for that.
        [TextArea] public string PriceText;
        [TextArea] public string AnmeldungText;
        [TextArea] public string SchufaText;
    }
}
