using System;
using Game.Scripts.PlayerModules;
using Game.Scripts.ZombieModules;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Night
{
    // Scene-local gate for the NightCity combat layer. Locks the player + spawner + combat
    // HUD on Awake so the player can't act and zombies aren't spawning while the NightIntro
    // overlay is up. Subscribes to GameManager.OnStateChanged and unlocks everything the moment
    // state becomes NightCombat (which GameManager sets after NightIntro is dismissed).
    public class NightCombatGate : MonoBehaviour
    {
        [Header("Spawning")]
        [SerializeField] private ZombieSpawnManager _spawnManager;

        [Header("Player modules (disabled until combat begins)")]
        [SerializeField] private PlayerMovementModule _playerMovement;
        [SerializeField] private PlayerShootingModule _playerShooting;

        [Header("Combat HUD")]
        [Tooltip("Parent of the gameplay menu (BottomGunUI, etc.) that should appear only once " +
                 "the player dismisses the night intro. Inactive in editor.")]
        [SerializeField] private GameObject _combatHUDRoot;

        private bool _combatStarted;

        private void Awake()
        {
            // Lock everything down — combat doesn't start until the intro closes.
            if (_playerMovement != null) _playerMovement.enabled = false;
            if (_playerShooting != null) _playerShooting.enabled = false;
            if (_combatHUDRoot != null) _combatHUDRoot.SetActive(false);
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged += HandleStateChanged;

            // Edge case: the gate was enabled AFTER the state had already moved to NightCombat
            // (e.g. a hot-reload). Catch it up so we don't sit locked forever.
            if (!_combatStarted && GameManager.Instance != null &&
                GameManager.Instance.State == GameState.NightCombat)
            {
                BeginCombat();
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown("space"))
            {
                BeginCombat();
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState prev, GameState next)
        {
            if (next == GameState.NightCombat && !_combatStarted) BeginCombat();
        }

        // Public so it can also be wired to a debug button or test hook.
        [Button]
        public void BeginCombat()
        {
            if (_combatStarted) return;
            _combatStarted = true;

            if (_playerMovement != null) _playerMovement.enabled = true;
            if (_playerShooting != null) _playerShooting.enabled = true;
            if (_combatHUDRoot != null) _combatHUDRoot.SetActive(true);
            if (_spawnManager != null) _spawnManager.StartWaves();
        }
    }
}
