using Game.Scripts.StudentData;

namespace Game.Scripts.Visa
{
    public enum VisaRequirement
    {
        Passport,
        LetterOfAdmission,
        FinancialProof,
        LanguageCertificate,
        ApsCertificate,
        VisaFeeReceipt,
        TravelInsurance,
        BiometricPhoto,
    }
    

    public static class VisaRules
    {
        // The correct answer for each requirement is the key imported into VisaData.
        public static CheckStatus Expected(VisaData visa, VisaRequirement requirement) => requirement switch
        {
            VisaRequirement.Passport => visa.CheckPassport,
            VisaRequirement.LetterOfAdmission => visa.CheckLetterOfAdmission,
            VisaRequirement.FinancialProof => visa.CheckBlockedAccount,
            VisaRequirement.LanguageCertificate => visa.CheckLanguageCertificate,
            VisaRequirement.ApsCertificate => visa.CheckAps,
            VisaRequirement.VisaFeeReceipt => visa.CheckVisaPaymentOrder,
            VisaRequirement.TravelInsurance => visa.CheckTravelInsurance,
            VisaRequirement.BiometricPhoto => visa.CheckBiometricPhotos,
            _ => CheckStatus.NotRequired,
        };
    }
}
