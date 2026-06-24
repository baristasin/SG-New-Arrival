using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public static class ZombieRegistry
    {
        private static readonly Dictionary<Collider, ZombieHealthModule> _map = new(512);

        public static void Register(Collider col, ZombieHealthModule health)
            => _map[col] = health;

        public static void Unregister(Collider col)
            => _map.Remove(col);

        public static bool TryGetHealth(Collider col, out ZombieHealthModule health)
            => _map.TryGetValue(col, out health);
        
        public static int Count => _map.Count;
    }
}
