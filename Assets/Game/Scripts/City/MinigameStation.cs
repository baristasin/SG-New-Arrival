using UnityEngine;

namespace Game.Scripts.City
{
    // One entry in the city's set of minigames. The root holds the station's camera, manager,
    // and minigame UI — toggled active when the player enters, inactive otherwise. The manager
    // reference is delegated to so GameManager can call BeginGame after the tutorial closes.
    public class MinigameStation : MonoBehaviour
    {
        [SerializeField] private MinigameId _id;
        [Tooltip("Root containing the station's camera + manager + minigame UI. Inactive in editor.")]
        [SerializeField] private GameObject _root;
        [Tooltip("MonoBehaviour on the station that implements IMinigameManager " +
                 "(AnmeldungManager, ApartmentHuntingManager, VisaManager).")]
        [SerializeField] private MonoBehaviour _manager;

        public MinigameId Id => _id;

        public void Enter() { if (_root != null) _root.SetActive(true); }
        public void Exit()  { if (_root != null) _root.SetActive(false); }

        // Trigger the minigame's actual round start (paper slide-in etc.). Called by GameManager
        // after EnterStation + tutorial dismissal.
        public void BeginGame()
        {
            if (_manager is IMinigameManager mg) { mg.BeginGame(); return; }
            Debug.LogWarning($"[MinigameStation] No IMinigameManager wired for {Id}.");
        }

        // Returns 0..100. Used by GameManager at end of day to feed the rewards screen.
        public int GetScorePercent()
        {
            if (_manager is IMinigameManager mg) return mg.GetScorePercent();
            return 0;
        }
    }
}
