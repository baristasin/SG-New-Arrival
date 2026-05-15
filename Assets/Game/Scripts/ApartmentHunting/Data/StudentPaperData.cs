using System;
using Game.Scripts.ApartmentHunting.Data;

namespace Game.Scripts.ApartmentHunting
{
    [Serializable]
    public class StudentPaperData
    {
        public string StudentName;
        public VisaStatus VisaStatus;
        public BudgetCategory Budget;
        public bool IsEnrolled;

        public string VisaStory;
        public string BudgetStory;
        public string EnrollmentStory;
    }
}
