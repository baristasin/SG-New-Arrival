namespace Game.Scripts.Anmeldung
{
    public enum PaperItemCategory
    {
        // Necessary fields — sourced from StudentProfile
        NameAndSurname = 0,
        Nationality = 1,
        LandlordCertificate = 2,
        Address = 3,
        MoveInDate = 4,

        // Distractor fields — never expected by any slot
        MonthlyIncome = 5,
        DateOfBirth = 6,
        SizeOfApartment = 7,
    }
}
