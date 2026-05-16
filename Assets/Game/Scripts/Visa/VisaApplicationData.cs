using System;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class VisaApplicationData
    {
        public PassportData Passport;
        public VisaApplyData VisaApply;
        public AdmissionData Admission;
        public FundsData Funds;
        public LanguageData Language;
        public APSData APS;
        public InsuranceData Insurance;
    }

    [Serializable]
    public class VisaApplicationListWrapper
    {
        public VisaApplicationData[] Items;
    }
}
