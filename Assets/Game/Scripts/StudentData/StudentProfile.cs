using System;

namespace Game.Scripts.StudentData
{
    [Serializable]
    public class StudentProfile
    {
        public int IdNumber;
        public string Id;
        public string FirstName;
        public string LastName;
        public string Nationality;
        public string PlaceOfBirth;
        public string DateOfBirth;
        public LanguageLevel GermanLevel;
        public LanguageLevel EnglishLevel;
        public Gender Gender;
        public MaritalStatus MaritalStatus;
        public PriceCategory Budget;
        public VisaStatus VisaStatus;
        public bool IsEnrolled;
        public string AddressInGermany;
        public string MoveInDate;
        public string Wohnungsgeber;
        public string FormerAddressAbroad;
        public bool HasPreviousSchufa;

        public VisaData Visa;

        public string FullName => $"{FirstName} {LastName}";
    }
}
