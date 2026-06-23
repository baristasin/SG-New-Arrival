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
    // Scene-local gate for the NightCity combat layer. Two responsibilities:
    //  1) Lock the player + spawner + HUD until NightIntro closes (state → NightCombat).
    //  2) Watch the player and building health modules; when either drops to 0, end combat —
    //     compute the next morning's starting sanity, show the result panel, then trigger
    //     GameManager.NightFinished() to return to day.
    public class NightCombatGate : MonoBehaviour
    {
        [Header("Spawning")]
        [SerializeField] private ZombieSpawnManager _spawnManager;

        [Header("Player modules (disabled until combat begins)")]
        [SerializeField] private PlayerMovementModule _playerMovement;
        [SerializeField] private PlayerShootingModule _playerShooting;

        [Header("Combat HUD")]
        [Tooltip("Parent of the gameplay menu (BottomGunUI, etc.) shown only once the night " +
                 "intro is dismissed. Inactive in editor.")]
        [SerializeField] private GameObject _combatHUDRoot;

        [Header("End condition watchers")]
        [SerializeField] private PlayerHealthModule _playerHealth;
        [SerializeField] private BuildingHealthModule _buildingHealth;

        [Header("Next-day sanity carry-over")]
        [Tooltip("Lowest sanity value the next morning can start with — even a flat-out failure " +
                 "of tonight's defence guarantees this much.")]
        [SerializeField, Range(0, 100)] private int _nextDaySanityFloor = 70;
        [Tooltip("Highest sanity the next morning can start with — reached when the building " +
                 "survives at full health.")]
        [SerializeField, Range(0, 100)] private int _nextDaySanityCeiling = 100;

        [Header("Survival clock")]
        [SerializeField] private NightClock _clock;
        [Tooltip("Hour the Ruhezeit (quiet hours) penalty kicks in. Loud weapons fired at or " +
                 "after this hour spawn extra zombies on a cooldown.")]
        [SerializeField] private int _ruhezeitStartHour = 22;

        [Header("Loud-weapon noise penalty")]
        [Tooltip("Extra zombies spawned per loud weapon fire during Ruhezeit.")]
        [SerializeField, Min(1)] private int _noisePenaltyCount = 2;
        [Tooltip("Cooldown between noise penalties so spamming a loud weapon doesn't flood " +
                 "the map.")]
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
                // Catch the case where startHour already meets the Ruhezeit threshold.
                _ruhezeitActive = _clock.Hour >= _ruhezeitStartHour;
            }

            // Tutorial dismissed → zombies summoned → night music starts.
            MusicController.Instance?.PlayNight();
        }

        // ── Survival clock hooks ──────────────────────────────────────────────────────────

        private void HandleHourPassed(int newHour)
        {
            if (!_ruhezeitActive && newHour >= _ruhezeitStartHour)
                _ruhezeitActive = true;
        }

        private void HandleNightEnded()
        {
            // Reached the end hour with player + building still alive → survival success.
            EndCombatSuccess();
        }

        // ── Noise penalty ─────────────────────────────────────────────────────────────────

        private void HandleWeaponFired(NoiseLevel noise)
        {
            if (!_combatStarted || _combatEnded) return;
            if (!_ruhezeitActive) return;
            if (noise != NoiseLevel.Loud) return;
            if (Time.time < _nextNoisePenaltyTime) return;

            _nextNoisePenaltyTime = Time.time + _noisePenaltyCooldown;
            if (_spawnManager != null) _spawnManager.SpawnExtra(_noisePenaltyCount);
        }

        // ── End condition ─────────────────────────────────────────────────────────────────

        private void HandlePlayerHealth(int hp)   { if (hp <= 0) EndCombatFail(); }
        private void HandleBuildingHealth(int hp) { if (hp <= 0) EndCombatFail(); }

        // Failure path — player or cathedral down. Tomorrow's sanity drops to a lerp between
        // floor and ceiling based on how much building HP survived.
        [Button]
        public void EndCombatFail()
        {
            if (_combatEnded || !_combatStarted) return;
            _combatEnded = true;

            int nextSanity = ComputeNextDaySanity();
            SanityManager.Instance?.SetNextStartingSanity(nextSanity);

            StartCoroutine(EndCombatFailRoutine(nextSanity));
        }

        // Success path — all waves cleared, both player and building alive. Wave logic calls
        // this when the last zombie is down; for now it's also exposed as a debug button.
        [Button]
        public void EndCombatSuccess()
        {
            if (_combatEnded || !_combatStarted) return;
            _combatEnded = true;

            // Successful nights leave morning sanity at full — no override means ResetSanity
            // falls back to MaxSanity.
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

            // Cut the night music — result panel + next-day load happen in silence.
            MusicController.Instance?.StopAll();
        }

        // Cathedral survival drives the next-day sanity — building at full = ceiling, building
        // gone = floor. Player death without building damage still leaves a strong morning.
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
