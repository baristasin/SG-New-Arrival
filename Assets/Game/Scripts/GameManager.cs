using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.City;
using Game.Scripts.SanityModules;
using Game.Scripts.TimeModules;
using Game.Scripts.UI;
using Game.Scripts.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts
{
    public enum GameState
    {
        Boot,             // pre-MainMenu, only Bootstrap singletons exist
        MainMenu,         // main menu shown
        LoadingCity,      // scene loading into DayCity
        DayStart,         // day intro card playing
        CityRoaming,      // player walks the city, DayClock running
        LoadingMinigame,  // brief transition into/out of a station (no scene load)
        Tutorial,         // first-time tutorial pages
        Minigame,         // a minigame is active
        Sleeping,         // sanity hit 0, eyelids closing
        DayRewards,       // end-of-day summary
        LoadingNight,     // scene loading into NightCity
        NightCombat,      // zombie combat scene
    }

    // Persistent game-flow orchestrator. Owns the day clock, tracks current day + state, wires
    // the main-menu buttons, drives the boot → day → minigame pipeline. Slices 5 & 6 finish the
    // sleep → rewards → night → next-day loop.
    public class GameManager : PersistentSingleton<GameManager>
    {
        [Header("Day clock tuning")]
        [Tooltip("In-game hour the city day starts on (e.g. 8 = 8:00 AM).")]
        [SerializeField] private int _dayStartHour = 8;
        [Tooltip("Past this hour the player starts losing sanity until they enter a minigame.")]
        [SerializeField] private int _missionDeadlineHour = 10;
        [Tooltip("Real seconds per in-game minute.")]
        [SerializeField] private float _secondsPerInGameMinute = 1f;

        [Header("Loading")]
        [Tooltip("Minimum time the loading screen stays up on a real scene load.")]
        [SerializeField] private float _minLoadingDuration = 2f;
        [Tooltip("Fixed cover duration for the in-scene minigame transition (no real load).")]
        [SerializeField] private float _minigameTransitionDuration = 0.6f;

        [Header("Day plan")]
        [Tooltip("Which minigame plays on day 1, 2, 3, ... in order. Indexed by day-1.")]
        [SerializeField] private List<MinigameId> _dayMinigames = new();

        public DayClock Clock { get; private set; }
        public int CurrentDay { get; private set; } = 1;
        public GameState State { get; private set; } = GameState.Boot;
        public CityHub CurrentCity { get; private set; }

        public int DayStartHour => _dayStartHour;
        public int MissionDeadlineHour => _missionDeadlineHour;

        // (previousState, newState)
        public event Action<GameState, GameState> OnStateChanged;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;        // a duplicate was destroyed by the base
            Clock = new DayClock { SecondsPerInGameMinute = _secondsPerInGameMinute };
        }

        private void Start()
        {
            WireMainMenu();
            ShowMainMenuOnBoot();
            WireSanity();
        }

        private void Update()
        {
            Clock?.Tick(Time.deltaTime);
        }

        // ── City registration ──────────────────────────────────────────────────

        public void RegisterCity(CityHub hub)   { CurrentCity = hub; }
        public void UnregisterCity(CityHub hub) { if (CurrentCity == hub) CurrentCity = null; }

        // ── Boot wiring ────────────────────────────────────────────────────────

        private void WireMainMenu()
        {
            var menu = UIManager.Instance?.MainMenu;
            if (menu == null) return;
            menu.OnNewGameClicked -= StartNewGame;
            menu.OnNewGameClicked += StartNewGame;
            menu.OnQuitClicked    -= QuitGame;
            menu.OnQuitClicked    += QuitGame;
        }

        private void ShowMainMenuOnBoot()
        {
            UIManager.Instance?.MainMenu?.Show();
            SetState(GameState.MainMenu);
        }

        private void WireSanity()
        {
            var sanity = SanityManager.Instance;
            if (sanity == null) return;
            sanity.OnSanityChanged -= HandleSanityChanged;
            sanity.OnSanityChanged += HandleSanityChanged;
        }

        // Sanity-0 detector. Fires the sleep-to-night pipeline once when sanity bottoms out
        // during a minigame or the city walk (late drain). EndDayBySleepRoutine sets state to
        // Sleeping immediately, so subsequent ticks early-return here.
        private void HandleSanityChanged(float normalized)
        {
            if (normalized > 0f) return;
            if (State != GameState.Minigame && State != GameState.CityRoaming) return;
            StartCoroutine(EndDayBySleepRoutine());
        }

        // ── Public flow API ────────────────────────────────────────────────────

        [Button]
        public void StartNewGame()
        {
            if (State != GameState.MainMenu) return;
            StartCoroutine(NewGameRoutine());
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        [Button]
        public void EnterMinigame(MinigameId id)
        {
            if (State != GameState.CityRoaming) return;
            StartCoroutine(EnterMinigameRoutine(id));
        }

        // Convenience: look up the planned minigame for CurrentDay and enter it. The production
        // path (mission door trigger) calls this; the parameterized EnterMinigame stays for tests.
        [Button]
        public void EnterTodaysMinigame()
        {
            var id = GetTodayMinigame();
            if (!id.HasValue)
            {
                Debug.LogWarning($"[GameManager] No minigame planned for day {CurrentDay}.");
                return;
            }
            EnterMinigame(id.Value);
        }

        public MinigameId? GetTodayMinigame()
        {
            int i = CurrentDay - 1;
            if (i < 0 || i >= _dayMinigames.Count) return null;
            return _dayMinigames[i];
        }

        // Called by the zombie combat layer (slice 6) when the night ends — all zombies down,
        // player dead, or cathedral destroyed. Triggers the post-night summary + next-day load.
        [Button]
        public void NightFinished()
        {
            if (State != GameState.NightCombat) return;
            StartCoroutine(NightFinishedRoutine());
        }

        // ── Flow coroutines ────────────────────────────────────────────────────

        private IEnumerator NewGameRoutine()
        {
            CurrentDay = 1;
            SetState(GameState.LoadingCity);

            var ui = UIManager.Instance;

            yield return ui.MainMenu.Hide().WaitForCompletion();
            yield return ui.Loading.Show().WaitForCompletion();

            // Clamp to a minimum duration so the screen doesn't blink off on fast loads.
            float t0 = Time.unscaledTime;
            yield return SceneLoader.Instance.LoadAsync(SceneLoader.DayCityScene);
            float remain = _minLoadingDuration - (Time.unscaledTime - t0);
            if (remain > 0f) yield return new WaitForSeconds(remain);

            yield return ui.Loading.Hide().WaitForCompletion();
            yield return BeginDay();
        }

        private IEnumerator BeginDay()
        {
            SetState(GameState.DayStart);

            var ui = UIManager.Instance;
            var sanity = SanityManager.Instance;

            sanity.ResetSanity();
            sanity.DrainPerSecond = 0f;          // no drain during the intro / early morning
            Clock.Reset(_dayStartHour);

            // Re-arm the late-drain hook for the new day (idempotent — guarded by state inside).
            Clock.OnHourPassed -= HandleHourPassedCity;
            Clock.OnHourPassed += HandleHourPassedCity;

            yield return ui.DayStart.Play(CurrentDay);

            ui.DayHUD.Show();
            Clock.Resume();
            SetState(GameState.CityRoaming);
        }

        private IEnumerator EnterMinigameRoutine(MinigameId id)
        {
            if (CurrentCity == null)
            {
                Debug.LogWarning("[GameManager] EnterMinigame called but no CityHub is registered.");
                yield break;
            }

            SetState(GameState.LoadingMinigame);
            Clock.Pause();
            SanityManager.Instance.DrainPerSecond = 0f;   // freeze drain across the transition

            var ui = UIManager.Instance;
            ui.DayHUD.Hide();                              // fire-and-forget; fades while loading shows
            yield return ui.Loading.Show().WaitForCompletion();
            yield return new WaitForSeconds(_minigameTransitionDuration);

            CurrentCity.EnterStation(id);

            yield return ui.Loading.Hide().WaitForCompletion();

            // First-time tutorial per minigame; flag persists via PlayerPrefs.
            if (!TutorialFlags.HasSeen(id))
            {
                SetState(GameState.Tutorial);
                yield return ui.Tutorial.Play(id);
                TutorialFlags.MarkSeen(id);
            }

            // Now that the player has read the tutorial (or skipped it on repeat plays), kick
            // off the actual minigame — paper slides in, screens slide in, etc.
            CurrentCity.GetStation(id)?.BeginGame();

            // Switch to minigame drain rate; current sanity carries over from city.
            var balance = BalanceVariables.Instance;
            if (balance != null)
                SanityManager.Instance.DrainPerSecond = balance.SanityDrainPerSecond;

            SetState(GameState.Minigame);
        }

        private IEnumerator EndDayBySleepRoutine()
        {
            SetState(GameState.Sleeping);
            SanityManager.Instance.DrainPerSecond = 0f;
            Clock.Pause();

            var ui = UIManager.Instance;
            ui.DayHUD.Hide();   // fire-and-forget; fades out while eyelids close

            // Eyelids close → explanation panel → wait for click
            yield return ui.EyeClose.Play();

            // Cover with loading, then hide the eye (its fade-out plays underneath).
            yield return ui.Loading.Show().WaitForCompletion();
            ui.EyeClose.Hide();

            SetState(GameState.LoadingNight);
            float t0 = Time.unscaledTime;
            yield return SceneLoader.Instance.LoadAsync(SceneLoader.NightCityScene);
            float remain = _minLoadingDuration - (Time.unscaledTime - t0);
            if (remain > 0f) yield return new WaitForSeconds(remain);

            yield return ui.Loading.Hide().WaitForCompletion();

            // One-click input-warning screen before combat actually starts. NightUI / GameHUD /
            // BottomGunUI live in the NightCity scene itself — already active behind this intro.
            yield return ui.NightIntro.Play();

            SetState(GameState.NightCombat);
            // Zombie fight runs here. The night layer calls NightFinished() when it's over.
        }

        private IEnumerator NightFinishedRoutine()
        {
            SetState(GameState.DayRewards);
            var ui = UIManager.Instance;

            yield return ui.DayRewards.Play(CurrentDay);

            CurrentDay++;
            SetState(GameState.LoadingCity);

            yield return ui.Loading.Show().WaitForCompletion();
            float t0 = Time.unscaledTime;
            yield return SceneLoader.Instance.LoadAsync(SceneLoader.DayCityScene);
            float remain = _minLoadingDuration - (Time.unscaledTime - t0);
            if (remain > 0f) yield return new WaitForSeconds(remain);
            yield return ui.Loading.Hide().WaitForCompletion();

            yield return BeginDay();
        }

        // Once 10:00 ticks past, start bleeding sanity until the player reaches a station.
        // Subsequent minigame entry overwrites DrainPerSecond with the minigame rate.
        private void HandleHourPassedCity(int hour)
        {
            if (State != GameState.CityRoaming) return;
            if (hour < _missionDeadlineHour) return;

            var balance = BalanceVariables.Instance;
            if (balance != null)
                SanityManager.Instance.DrainPerSecond = balance.LateDrainPerSecond;
        }

        // ── State helpers ──────────────────────────────────────────────────────

        protected void SetState(GameState next)
        {
            if (State == next) return;
            var prev = State;
            State = next;
            OnStateChanged?.Invoke(prev, next);
        }
    }
}
