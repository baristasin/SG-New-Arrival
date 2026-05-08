using UnityEngine;

namespace Game.Scripts.Utilities
{
    public static class PlayerReference
    {
        public static Transform Transform { get; private set; }
        public static Vector3 Position => Transform.position;
        public static float X => Transform.position.x;
        public static float Z => Transform.position.z;

        public static void Set(Transform player)
        {
            Transform = player;
        }
    }
}
