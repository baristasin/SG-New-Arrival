using UnityEngine;

namespace Game.Scripts.GunModules
{
    public enum WeaponCategory { PlayerHeld, Turret }

    public enum NoiseLevel { Quiet, Medium, Loud }

    [CreateAssetMenu(menuName = "Weapons/Weapon Data", fileName = "WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string DisplayName;
        public int RewardDay = 1;
        public WeaponCategory Category = WeaponCategory.PlayerHeld;

        public int Damage = 5;
        public float Range = 10f;
        public float FireInterval = 0.3f;   // seconds between shots / hits
        public float Knockback = 0f;
        public NoiseLevel Noise = NoiseLevel.Medium;

        public int MagazineSize = 0;
        public float ReloadTime = 1f;
    }
}
