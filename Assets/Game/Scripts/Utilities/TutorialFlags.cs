using System;
using UnityEngine;

namespace Game.Scripts.Utilities
{
    // Tracks whether the per-minigame fullscreen tutorial has been shown. Persists across sessions
    // via PlayerPrefs for now; when the save system arrives, swap this store and keep the API.
    public static class TutorialFlags
    {
        private const string KeyPrefix = "Tutorial.";

        public static bool HasSeen(MinigameId id) =>
            PlayerPrefs.GetInt(KeyPrefix + id, 0) == 1;

        public static void MarkSeen(MinigameId id)
        {
            PlayerPrefs.SetInt(KeyPrefix + id, 1);
            PlayerPrefs.Save();
        }

        public static void ResetAll()
        {
            foreach (MinigameId id in Enum.GetValues(typeof(MinigameId)))
                PlayerPrefs.DeleteKey(KeyPrefix + id);
            PlayerPrefs.Save();
        }
    }
}
