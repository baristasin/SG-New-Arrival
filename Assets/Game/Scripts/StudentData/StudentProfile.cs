using System;
using UnityEngine;

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

        // Exchange (3-month) vs full master's programme — drives the Length-of-Stay line on the
        // student paper. True = exchange, false = master's.
        public bool IsExchangeStudent;

        // Free-form flavour text per student — Budget is fully unique, so it stays as data.
        [TextArea] public string BudgetText;

        public VisaData Visa;

        public string FullName => $"{FirstName} {LastName}";
    }
}
