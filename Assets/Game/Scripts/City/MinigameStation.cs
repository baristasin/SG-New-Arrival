using UnityEngine;

namespace Game.Scripts.City
{

    public class MinigameStation : MonoBehaviour
    {
        [SerializeField] private MinigameId _id;
        [SerializeField] private GameObject _root;
        [SerializeField] private MonoBehaviour _manager;

        public MinigameId Id => _id;

        public void Enter() { if (_root != null) _root.SetActive(true); }
        public void Exit()  { if (_root != null) _root.SetActive(false); }

        public void BeginGame()
        {
            if (_manager is IMinigameManager mg) { mg.BeginGame(); return; }
            Debug.LogWarning($"[MinigameStation] No IMinigameManager wired for {Id}.");
        }

        public int GetScorePercent()
        {
            if (_manager is IMinigameManager mg) return mg.GetScorePercent();
            return 0;
        }
    }
}
