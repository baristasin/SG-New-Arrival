using System;

namespace Game.Scripts.StudentData
{
    // Visa-application data for a student, sourced from the orange columns (S–AO) of the
    // student profiles spreadsheet. Dates are kept as ISO "yyyy-MM-dd" strings (empty when blank),
    // matching the convention used by StudentProfile.DateOfBirth / MoveInDate.
    [Serializable]
    public class VisaData
    {
        // Passport
        public string PassportId;
        public string PassportIssued;
        public string PassportExpiryDate;

        // Admission & funds
        public string AdmissionDeadline;
        public int FinancialFunds;
        public int MonthlyRelease;
        public string Iban;

        // Language certificate
        public LanguageLevel LanguageCertLevel;
        public string LanguageCertDate;
        public string LanguageCertProvider;

        // Other documents
        public string ApsName;
        public float VisaPaymentOrder;
        public string TravelInsurancePerson;

        // Biometric photos
        public int PhotosTotal;
        public PhotoQuality PhotosQuality;

        // Review checks (tri-state: Yes / No / NotRequired)
        public CheckStatus CheckPassport;
        public CheckStatus CheckLetterOfAdmission;
        public CheckStatus CheckBlockedAccount;
        public CheckStatus CheckLanguageCertificate;
        public CheckStatus CheckAps;
        public CheckStatus CheckVisaPaymentOrder;
        public CheckStatus CheckTravelInsurance;
        public CheckStatus CheckBiometricPhotos;
    }
}
