using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.City
{
    // Scene-local controller for the DayCity scene. Registers itself with GameManager on enable
    // so the persistent flow can drive it. Owns the list of minigame stations and toggles them
    // on/off. Camera switching is handled by Cinemachine Brain via vcam priorities — activating
    // a station root brings its CinemachineCamera online and the brain blends to it; no need to
    // touch the city camera ourselves.
    public class CityHub : MonoBehaviour
    {
        [SerializeField] private List<MinigameStation> _stations = new();
        [SerializeField] private GameObject _player;

        private void OnEnable()  { GameManager.Instance?.RegisterCity(this); }
        private void OnDisable() { GameManager.Instance?.UnregisterCity(this); }

        public MinigameStation GetStation(MinigameId id) =>
            _stations.Find(s => s != null && s.Id == id);

        // Disable every other station and enable the target one. Cinemachine handles the camera.
        // Player is hidden so the city body isn't visible behind the minigame.
        public void EnterStation(MinigameId id)
        {
            foreach (var s in _stations) if (s != null) s.Exit();

            var station = GetStation(id);
            if (station == null)
            {
                Debug.LogWarning($"[CityHub] No station registered for id {id}.");
                return;
            }

            if (_player != null) _player.SetActive(false);
            station.Enter();
        }

        // Reverse — all stations off; Cinemachine blends back to the city camera, player visible.
        public void ExitToCity()
        {
            foreach (var s in _stations) if (s != null) s.Exit();
            if (_player != null) _player.SetActive(true);
        }
    }
}
