using UnityEngine;

namespace Game.Scripts.City
{
    [RequireComponent(typeof(Collider))]
    public class MissionTrigger : MonoBehaviour
    {
        [SerializeField] private MinigameId _minigameId;
        [SerializeField] private bool _onlyIfTodaysMinigame = true;
        [SerializeField] private string _playerTag = "Player";

        public MinigameId Id => _minigameId;

        private bool _fired;

        private void Reset()
        {

            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_fired) return;
            if (!other.CompareTag(_playerTag)) return;

            var gm = GameManager.Instance;
            if (gm == null) return;
            if (gm.State != GameState.CityRoaming) return;

            if (_onlyIfTodaysMinigame)
            {
                var today = gm.GetTodayMinigame();
                if (!today.HasValue || today.Value != _minigameId) return;
            }

            _fired = true;
            gm.EnterMinigame(_minigameId);
        }
    }
}
