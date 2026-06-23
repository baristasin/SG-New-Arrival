namespace Game.Scripts
{
    // Common contract for the three day-cycle minigames. GameManager calls BeginGame after the
    // station camera + root are activated AND any first-time tutorial has been dismissed, so the
    // paper / items / screens can slide in cleanly behind a "ready to play" moment.
    //
    // GetScorePercent returns the run's score as 0..100 — number of correct answers / actions
    // versus a required count, capped at 100 (so over-performing doesn't go past 100 but still
    // covers any earlier mistakes).
    public interface IMinigameManager
    {
        void BeginGame();
        int GetScorePercent();
    }
}
