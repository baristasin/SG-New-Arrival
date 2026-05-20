namespace Game.Scripts.Anmeldung
{
    public enum PaperItemCategory
    {
        // Necessary fields — sourced from StudentProfile
        NameAndSurname = 0,
        Nationality = 1,
        DateOfBirth = 2,
        Address = 3,
        MoveInDate = 4,

        // Distractor fields — randomly generated, never expected by any slot
        MonthlyIncome = 5,
        Employer = 6,
        SizeOfApartment = 7,
    }
}
