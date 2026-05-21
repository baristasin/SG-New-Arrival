using System;
using System.Collections.Generic;
using System.Globalization;
using Game.Scripts.StudentData;
using UnityEngine;

namespace Game.Scripts.Anmeldung
{
    public static class AnmeldungItemFactory
    {
        public static List<DraggableItemData> BuildRound(StudentProfile student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            var items = new List<DraggableItemData>
            {
                Make(PaperItemCategory.NameAndSurname, student.FullName),
                Make(PaperItemCategory.Nationality, student.Nationality),
                Make(PaperItemCategory.LandlordCertificate, student.Wohnungsgeber),
                Make(PaperItemCategory.Address, student.AddressInGermany),
                Make(PaperItemCategory.MoveInDate, FormatGermanDate(student.MoveInDate)),

                Make(PaperItemCategory.MonthlyIncome, $"€{UnityEngine.Random.Range(600, 2001)}"),
                Make(PaperItemCategory.DateOfBirth, FormatGermanDate(student.DateOfBirth)),
                Make(PaperItemCategory.SizeOfApartment, $"{UnityEngine.Random.Range(20, 51)} m²"),
            };

            Shuffle(items);
            return items;
        }

        private static DraggableItemData Make(PaperItemCategory category, string text) =>
            new() { PaperItemCategory = category, ItemDataStr = text };

        private static string FormatGermanDate(string isoDate)
        {
            if (string.IsNullOrEmpty(isoDate)) return string.Empty;
            if (DateTime.TryParseExact(isoDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var dt))
                return dt.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            return isoDate;
        }

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
