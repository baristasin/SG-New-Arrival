using System.Collections;
using Game.Scripts.AudioModules;
using Game.Scripts.BuildingModules;
using Game.Scripts.GunModules;
using Game.Scripts.PlayerModules;
using Game.Scripts.SanityModules;
using Game.Scripts.UI;
using Game.Scripts.ZombieModules;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Night
{
    public class NightCombatGate : MonoBehaviour
    {
        [SerializeField] private ZombieSpawnManager _spawnManager;

        [SerializeField] private PlayerMovementModule _playerMovement;
        [SerializeField] private PlayerShootingModule _playerShooting;

        [SerializeField] private GameObject _combatHUDRoot;

        [SerializeField] private PlayerHealthModule _playerHealth;
        [SerializeField] private BuildingHealthModule _buildingHealth;

        [SerializeField, Range(0, 100)] private int _nextDaySanityFloor = 70;
        [SerializeField, Range(0, 100)] private int _nextDaySanityCeiling = 100;

        [SerializeField] private NightClock _clock;
        [SerializeField] private int _ruhezeitStartHour = 22;

        [SerializeField, Min(1)] private int _noisePenaltyCount = 2;
        [SerializeField, Min(0f)] private float _noisePenaltyCooldown = 2.5f;

        private bool _combatStarted;
        private bool _combatEnded;
        private bool _ruhezeitActive;
        private float _nextNoisePenaltyTime;

        private void Awake()
        {
            if (_playerMovement != null) _playerMovement.enabled = false;
            if (_playerShooting != null) _playerShooting.enabled = false;
            if (_combatHUDRoot != null) _combatHUDRoot.SetActive(false);
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged += HandleStateChanged;

            if (_playerHealth != null)   _playerHealth.OnHealthChanged   += HandlePlayerHealth;
            if (_buildingHealth != null) _buildingHealth.OnHealthChanged += HandleBuildingHealth;

            if (_clock != null)
            {
                _clock.OnHourPassed += HandleHourPassed;
                _clock.OnEndReached += HandleNightEnded;
            }

            WeaponBase.OnFired += HandleWeaponFired;

            if (!_combatStarted && GameManager.Instance != null &&
                GameManager.Instance.State == GameState.NightCombat)
            {
                BeginCombat();
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;

            if (_playerHealth != null)   _playerHealth.OnHealthChanged   -= HandlePlayerHealth;
            if (_buildingHealth != null) _buildingHealth.OnHealthChanged -= HandleBuildingHealth;

            if (_clock != null)
            {
                _clock.OnHourPassed -= HandleHourPassed;
                _clock.OnEndReached -= HandleNightEnded;
            }

            WeaponBase.OnFired -= HandleWeaponFired;
        }

        private void HandleStateChanged(GameState prev, GameState next)
        {
            if (next == GameState.NightCombat && !_combatStarted) BeginCombat();
        }

        [Button]
        public void BeginCombat()
        {
            if (_combatStarted) return;
            _combatStarted = true;

            if (_playerMovement != null) _playerMovement.enabled = true;
            if (_playerShooting != null) _playerShooting.enabled = true;
            if (_combatHUDRoot != null) _combatHUDRoot.SetActive(true);
            if (_spawnManager != null) _spawnManager.StartWaves();

            if (_clock != null)
            {
                _clock.ResetClock();
                _clock.StartClock();
                _ruhezeitActive = _clock.Hour >= _ruhezeitStartHour;
            }

            MusicController.Instance?.PlayNight();
        }


        private void HandleHourPassed(int newHour)
        {
            if (!_ruhezeitActive && newHour >= _ruhezeitStartHour)
                _ruhezeitActive = true;
        }

        private void HandleNightEnded()
        {
            EndCombatSuccess();
        }
        
        private void HandleWeaponFired(NoiseLevel noise)
        {
            if (!_combatStarted || _combatEnded) return;
            if (!_ruhezeitActive) return;
            if (noise != NoiseLevel.Loud) return;
            if (Time.time < _nextNoisePenaltyTime) return;

            _nextNoisePenaltyTime = Time.time + _noisePenaltyCooldown;
            if (_spawnManager != null) _spawnManager.SpawnExtra(_noisePenaltyCount);
        }


        private void HandlePlayerHealth(int hp)   { if (hp <= 0) EndCombatFail(); }
        private void HandleBuildingHealth(int hp) { if (hp <= 0) EndCombatFail(); }


        [Button]
        public void EndCombatFail()
        {
            if (_combatEnded || !_combatStarted) return;
            _combatEnded = true;

            int nextSanity = ComputeNextDaySanity();
            SanityManager.Instance?.SetNextStartingSanity(nextSanity);

            StartCoroutine(EndCombatFailRoutine(nextSanity));
        }


        [Button]
        public void EndCombatSuccess()
        {
            if (_combatEnded || !_combatStarted) return;
            _combatEnded = true;


            int kills = ZombieSpawnManager.KillCount;

            StartCoroutine(EndCombatSuccessRoutine(kills));
        }

        private IEnumerator EndCombatFailRoutine(int nextSanity)
        {
            FreezePlayer();
            var resultUI = UIManager.Instance != null ? UIManager.Instance.NightResult : null;
            if (resultUI != null) yield return resultUI.PlayFail(nextSanity);
            else                   yield return null;
            GameManager.Instance?.NightFinished();
        }

        private IEnumerator EndCombatSuccessRoutine(int kills)
        {
            FreezePlayer();
            var resultUI = UIManager.Instance != null ? UIManager.Instance.NightResult : null;
            if (resultUI != null) yield return resultUI.PlaySuccess(kills);
            else                   yield return null;
            GameManager.Instance?.NightFinished();
        }

        private void FreezePlayer()
        {
            if (_playerMovement != null) _playerMovement.enabled = false;
            if (_playerShooting != null) _playerShooting.enabled = false;

            MusicController.Instance?.StopAll();
        }
        
        private int ComputeNextDaySanity()
        {
            float pct = 0f;
            if (_buildingHealth != null && _buildingHealth.MaxHealth > 0)
                pct = Mathf.Clamp01((float)_buildingHealth.CurrentHealth / _buildingHealth.MaxHealth);

            int value = Mathf.RoundToInt(Mathf.Lerp(_nextDaySanityFloor, _nextDaySanityCeiling, pct));
            return Mathf.Clamp(value, _nextDaySanityFloor, _nextDaySanityCeiling);
        }
    }
}
