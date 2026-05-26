namespace Game.Scripts.SanityModules
{
    // Higher = worse. Minigames add more corruption per stage.
    public enum SanityStage
    {
        Stable,      // >= StableThreshold (default 75)
        Unsettled,   // >= UnsettledThreshold (50)
        Disturbed,   // >= DisturbedThreshold (25)
        Critical,    // below DisturbedThreshold
    }
}
