namespace Game.Scripts.GunModules.Turrets
{
    // Pfandmaschine: fires homing bottles on a fixed (fast) cadence (WeaponData.FireInterval),
    // with a small splash on impact (set _areaRadius > 0 in the inspector).
    public class PfandmaschineTurret : ProjectileTurretBase
    {
        protected override bool TryFire() => LaunchAtNearest() != null;
    }
}
