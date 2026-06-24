namespace Game.Scripts.SanityModules
{
    public enum SanityStage
    {
        Stable,      // >= StableThreshold (default 75)
        Unsettled,   // >= UnsettledThreshold (50)
        Disturbed,   // >= DisturbedThreshold (25)
        Critical,    // below DisturbedThreshold
    }
}
