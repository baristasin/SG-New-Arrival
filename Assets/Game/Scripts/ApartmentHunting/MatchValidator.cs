using Game.Scripts.StudentData;

namespace Game.Scripts.ApartmentHunting
{
    public struct MatchResult
    {
        public bool PriceMatch;
        public bool AnmeldungMatch;
        public bool DormitoryMatch;
        public bool SchufaMatch;

        public int CorrectCount => (PriceMatch ? 1 : 0) + (AnmeldungMatch ? 1 : 0) + (DormitoryMatch ? 1 : 0);
        public int TotalRules => 3;
        public bool IsFullMatch => CorrectCount == TotalRules;
    }

    public static class MatchValidator
    {
        public static MatchResult Evaluate(StudentProfile student, ApartmentEntry apartment)
        {
            return new MatchResult
            {
                PriceMatch = IsPriceMatch(student, apartment),
                AnmeldungMatch = IsAnmeldungMatch(student, apartment),
                DormitoryMatch = IsDormitoryMatch(student, apartment),
                SchufaMatch = IsSchufaMatch(student, apartment)
            };
        }

        private static bool IsPriceMatch(StudentProfile student, ApartmentEntry apartment)
        {
            return student.Budget >= apartment.PriceCategory;
        }

        // Long-term students must register their address (Anmeldung), which requires the
        // landlord's Wohnungsgeberbescheinigung. Short-term stays skip registration.
        private static bool IsAnmeldungMatch(StudentProfile student, ApartmentEntry apartment)
        {
            if (student.VisaStatus == VisaStatus.LongTerm)
                return apartment.ProvidesWohnungsgeberbescheinigung;
            return true;
        }

        // Dormitories are reserved for enrolled students.
        private static bool IsDormitoryMatch(StudentProfile student, ApartmentEntry apartment)
        {
            if (apartment.Type == ApartmentType.Dormitory)
                return student.IsEnrolled;
            return true;
        }

        private static bool IsSchufaMatch(StudentProfile student, ApartmentEntry apartment)
        {
            if (apartment.RequiresSchufa)
            {
                return student.HasPreviousSchufa;
            }

            return true;
        }
    }
}
