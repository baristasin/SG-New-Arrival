using UnityEngine;

namespace Game.Scripts.City
{
    // Trigger volume placed at the entrance of a minigame station. When the player walks in,
    // it calls GameManager.EnterMinigame for its MinigameId — but only while the player is
    // roaming the city AND (optionally) the id matches the day's planned minigame, so the
    // "one minigame per day" rule isn't broken by walking into the wrong door.
    //
    // Fires once per scene load (re-armed automatically because DayCity reloads after night).
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
            // Ensure the attached collider is a trigger so the GameObject behaves correctly
            // straight out of Add Component.
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
