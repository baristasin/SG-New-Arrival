namespace Game.Scripts
{
    // Common contract for the three day-cycle minigames. GameManager calls BeginGame after the
    // station camera + root are activated AND any first-time tutorial has been dismissed, so the
    // paper / items / screens can slide in cleanly behind a "ready to play" moment.
    public interface IMinigameManager
    {
        void BeginGame();
    }
}
