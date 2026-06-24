using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.StudentData.Editor
{
    public static class StudentDataImporter
    {
        private const string StudentsJsonPath = "Assets/Game/Data/Source/students.json";
        private const string ApartmentsJsonPath = "Assets/Game/Data/Source/apartments.json";
        private const string StudentDbPath = "Assets/Game/Data/StudentDatabase.asset";
        private const string ApartmentDbPath = "Assets/Game/Data/ApartmentDatabase.asset";

        [MenuItem("Tools/Student Data/Generate Databases from JSON")]
        public static void GenerateDatabases()
        {
            ImportStudents();
            ImportApartments();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Student Data/Ping Student Database")]
        public static void PingStudentDb()
        {
            var db = AssetDatabase.LoadAssetAtPath<StudentDatabase>(StudentDbPath);
            if (db != null) EditorGUIUtility.PingObject(db);
        }

        [MenuItem("Tools/Student Data/Ping Apartment Database")]
        public static void PingApartmentDb()
        {
            var db = AssetDatabase.LoadAssetAtPath<ApartmentDatabase>(ApartmentDbPath);
            if (db != null) EditorGUIUtility.PingObject(db);
        }

        private static void ImportStudents()
        {
            var json = ReadText(StudentsJsonPath);
            var wrapper = JsonUtility.FromJson<StudentListWrapper>(json);

            var db = LoadOrCreate<StudentDatabase>(StudentDbPath);
            db.Students.Clear();
            db.Students.AddRange(wrapper.Items);
            EditorUtility.SetDirty(db);
        }

        private static void ImportApartments()
        {
            var json = ReadText(ApartmentsJsonPath);
            var wrapper = JsonUtility.FromJson<ApartmentListWrapper>(json);

            var db = LoadOrCreate<ApartmentDatabase>(ApartmentDbPath);
            db.Apartments.Clear();
            db.Apartments.AddRange(wrapper.Items);
            EditorUtility.SetDirty(db);
        }

        private static string ReadText(string assetPath)
        {
            var absolute = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
            if (!File.Exists(absolute))
                throw new FileNotFoundException($"Missing source JSON: {assetPath}");
            return File.ReadAllText(absolute);
        }

        private static T LoadOrCreate<T>(string assetPath) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null) return asset;

            var dir = Path.GetDirectoryName(assetPath);
            if (!string.IsNullOrEmpty(dir) && !AssetDatabase.IsValidFolder(dir))
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), dir));

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
            return asset;
        }

        [System.Serializable]
        private class StudentListWrapper
        {
            public System.Collections.Generic.List<StudentProfile> Items;
        }

        [System.Serializable]
        private class ApartmentListWrapper
        {
            public System.Collections.Generic.List<ApartmentEntry> Items;
        }
    }
}
