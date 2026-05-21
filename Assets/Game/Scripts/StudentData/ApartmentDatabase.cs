using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.StudentData
{
    [CreateAssetMenu(menuName = "StudentData/Apartment Database", fileName = "ApartmentDatabase")]
    public class ApartmentDatabase : ScriptableObject
    {
        [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
        public List<ApartmentEntry> Apartments = new();

        public ApartmentEntry FindById(int id) => Apartments.Find(a => a.Id == id);
    }
}
