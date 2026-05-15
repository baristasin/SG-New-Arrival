using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.ApartmentHunting.Data
{
    public static class ApartmentHuntingDataLoader
    {
        private const string ApartmentsPath = "ApartmentHunting/apartments";
        private const string StudentsPath = "ApartmentHunting/students";

        public static List<ApartmentPaperData> LoadApartments()
        {
            var json = Resources.Load<TextAsset>(ApartmentsPath);
            return JsonUtility.FromJson<ApartmentListWrapper>(json.text).Items;
        }

        public static List<StudentPaperData> LoadStudents()
        {
            var json = Resources.Load<TextAsset>(StudentsPath);
            return JsonUtility.FromJson<StudentListWrapper>(json.text).Items;
        }

        [System.Serializable]
        private class ApartmentListWrapper
        {
            public List<ApartmentPaperData> Items;
        }

        [System.Serializable]
        private class StudentListWrapper
        {
            public List<StudentPaperData> Items;
        }
    }
}
