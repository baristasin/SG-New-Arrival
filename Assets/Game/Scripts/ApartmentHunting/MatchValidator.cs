using Game.Scripts.ApartmentHunting.Data;

namespace Game.Scripts.ApartmentHunting
{
    public struct MatchResult
    {
        public bool VisaMatch;
        public bool BudgetMatch;
        public bool EnrollmentMatch;

        public int CorrectCount => (VisaMatch ? 1 : 0) + (BudgetMatch ? 1 : 0) + (EnrollmentMatch ? 1 : 0);
        public int TotalRules => 3;
        public bool IsFullMatch => CorrectCount == TotalRules;
    }

    public static class MatchValidator
    {
        public static MatchResult Evaluate(StudentPaperData student, ApartmentPaperData apartment)
        {
            return new MatchResult
            {
                VisaMatch = IsVisaMatch(student, apartment),
                BudgetMatch = IsBudgetMatch(student, apartment),
                EnrollmentMatch = IsEnrollmentMatch(student, apartment)
            };
        }

        private static bool IsVisaMatch(StudentPaperData student, ApartmentPaperData apartment)
        {
            return student.VisaStatus == VisaStatus.LongTerm
                ? apartment.ProvidesAnmeldung
                : !apartment.ProvidesAnmeldung;
        }

        private static bool IsBudgetMatch(StudentPaperData student, ApartmentPaperData apartment)
        {
            return student.Budget >= apartment.PriceCategory;
        }

        private static bool IsEnrollmentMatch(StudentPaperData student, ApartmentPaperData apartment)
        {
            if (apartment.IsDormitory && !student.IsEnrolled) return false;
            return true;
        }
    }
}
