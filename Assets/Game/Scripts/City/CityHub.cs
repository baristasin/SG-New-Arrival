using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.City
{

    public class CityHub : MonoBehaviour
    {
        [SerializeField] private List<MinigameStation> _stations = new();
        [SerializeField] private GameObject _player;

        private void OnEnable()  { GameManager.Instance?.RegisterCity(this); }
        private void OnDisable() { GameManager.Instance?.UnregisterCity(this); }

        public MinigameStation GetStation(MinigameId id) =>
            _stations.Find(s => s != null && s.Id == id);


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

        public void SetPlayerActive(bool active)
        {
            if (_player != null) _player.SetActive(active);
        }

        public void ExitToCity()
        {
            foreach (var s in _stations) if (s != null) s.Exit();
            if (_player != null) _player.SetActive(true);
        }
    }
}
