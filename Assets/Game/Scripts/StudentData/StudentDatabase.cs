using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.StudentData
{
    [CreateAssetMenu(menuName = "StudentData/Student Database", fileName = "StudentDatabase")]
    public class StudentDatabase : ScriptableObject
    {
        [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
        public List<StudentProfile> Students = new();

        public StudentProfile FindById(string id) => Students.Find(s => s.Id == id);
    }
}
