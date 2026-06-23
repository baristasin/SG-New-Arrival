using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.AudioModules;
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
        [Tooltip("Loading bar fill duration for routine transitions (city↔minigame, sleep→night, " +
                 "night→day).")]
        [SerializeField] private float _minLoadingDuration = 3f;
        [Tooltip("Longer loading bar duration for the first NewGame load — gives the player " +
                 "time to mash the Complain button.")]
        [SerializeField] private float _newGameLoadingDuration = 6f;
        [Tooltip("Legacy field — no longer used; the minigame transition runs through FillBar.")]
        [SerializeField] private float _minigameTransitionDuration = 0.6f;

        [Header("Day plan")]
        [Tooltip("Which minigame plays on day 1, 2, 3, ... in order. Indexed by day-1.")]
        [SerializeField] private List<MinigameId> _dayMinigames = new();

        [Header("Dev")]
        [Tooltip("DEV ONLY: when checked, NewGame skips straight to NightCity on the given day. " +
                 "Use it to test the night build (all weapons / turrets unlocked) without playing " +
                 "the full day cycle.")]
        [SerializeField] private bool _devSkipToNight;
        [SerializeField, Range(1, 10)] private int _devSkipDay = 4;

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
            Application.targetFrameRate = 30;
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
            // Day music runs across the menu too so the boot screen isn't silent.
            MusicController.Instance?.PlayDay();
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
            if (_devSkipToNight)
                StartCoroutine(DevSkipToNightRoutine());
            else
                StartCoroutine(NewGameRoutine());
        }

        // DEV ONLY: pretend the player already lived through _devSkipDay-1 days and jump straight
        // into the night combat scene with the corresponding wave + weapons unlocked.
        private IEnumerator DevSkipToNightRoutine()
        {
            CurrentDay = Mathf.Max(1, _devSkipDay);
            SetState(GameState.LoadingNight);

            var ui = UIManager.Instance;
            ui.Loading.Show();
            yield return ui.MainMenu.Hide().WaitForCompletion();

            // Stop the day music — night music kicks in when NightCombatGate.BeginCombat fires.
            MusicController.Instance?.StopAll();

            StartCoroutine(SceneLoader.Instance.LoadAsync(SceneLoader.NightCityScene));
            yield return ui.Loading.FillBar(_minLoadingDuration);

            // NightIntro fades in over Loading → hide Loading once shown → wait click → fade out.
            yield return ui.NightIntro.Play(onShown: () => ui.Loading.Hide());

            // SetState(NightCombat) triggers NightCombatGate.BeginCombat via OnStateChanged,
            // which starts the spawner + night music + unlocks the player input.
            SetState(GameState.NightCombat);
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
            if (_dayMinigames == null || _dayMinigames.Count == 0) return null;
            // Wrap so Day N+1 → first minigame, etc. — keeps the rotation going past the list end.
            int i = ((CurrentDay - 1) % _dayMinigames.Count + _dayMinigames.Count) % _dayMinigames.Count;
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

            // Cover with Loading FIRST (instant) so MainMenu's fade-out happens behind cover —
            // no scene/skybox flash during the gap.
            ui.Loading.Show();
            yield return ui.MainMenu.Hide().WaitForCompletion();

            // Real scene load runs in parallel; the bar fills over _newGameLoadingDuration so
            // the player has 5-6 seconds to mash the Complain button.
            StartCoroutine(SceneLoader.Instance.LoadAsync(SceneLoader.DayCityScene));
            yield return ui.Loading.FillBar(_newGameLoadingDuration);

            // BeginDay shows DayStart over Loading, then hides Loading once DayStart fully covers.
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

            // DayStart fades in OVER Loading; once it fully covers, hide Loading underneath
            // (instant, invisible to player). DayStart then waits for click and fades out,
            // revealing DayCity cleanly.
            yield return ui.DayStart.Play(CurrentDay, onShown: () => ui.Loading.Hide());

            ui.DayHUD.Show();
            Clock.Resume();

            // Start (or keep) the daytime music loop. Idempotent — no-op if already playing.
            MusicController.Instance?.PlayDay();

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

            // Cover with Loading FIRST so DayHUD's fade-out and the camera switch happen behind cover.
            ui.Loading.Show();
            ui.DayHUD.Hide();   // fire-and-forget; fades behind Loading
            CurrentCity.EnterStation(id);
            yield return ui.Loading.FillBar(_minLoadingDuration);

            // First-time tutorial: fades in over Loading → hide Loading once shown → wait click
            // → tutorial fades out, revealing minigame scene.
            if (!TutorialFlags.HasSeen(id))
            {
                SetState(GameState.Tutorial);
                yield return ui.Tutorial.Play(id, onShown: () => ui.Loading.Hide());
                TutorialFlags.MarkSeen(id);
            }
            else
            {
                // No tutorial this time — just drop the cover; the minigame's slide-in carries
                // the visual continuity.
                ui.Loading.Hide();
            }

            // Now that the player has read the tutorial (or skipped it on repeat plays), kick
            // off the actual minigame — paper slides in, screens slide in, etc.
            CurrentCity.GetStation(id)?.BeginGame();

            // Switch to minigame drain rate; current sanity carries over from city. Per-minigame
            // overrides in BalanceVariables let one minigame burn down faster than another.
            var balance = BalanceVariables.Instance;
            if (balance != null)
                SanityManager.Instance.DrainPerSecond = balance.GetMinigameDrain(id);

            SetState(GameState.Minigame);
        }

        private IEnumerator EndDayBySleepRoutine()
        {
            SetState(GameState.Sleeping);
            SanityManager.Instance.DrainPerSecond = 0f;
            Clock.Pause();

            // Cut the day music — NightIntro plays in silence, night music kicks in on combat start.
            MusicController.Instance?.StopAll();

            var ui = UIManager.Instance;
            ui.DayHUD.Hide();   // fire-and-forget; fades out behind the eyelids

            // Eyelids close → narrative explanation → wait for click. EyeClose stays "shown" after Play.
            yield return ui.EyeClose.Play();

            // Pull the score from whichever station the player was just playing.
            int score = 0;
            var todayId = GetTodayMinigame();
            var station = (todayId.HasValue && CurrentCity != null) ? CurrentCity.GetStation(todayId.Value) : null;
            if (station != null) score = station.GetScorePercent();
            Debug.Log($"[GameManager] Sleep score lookup: todayId={todayId} city={CurrentCity} station={station} score={score}");

            // Today's reward card on top of the closed eye. On dismiss we raise Loading (instant)
            // and fade EyeClose out in parallel with DayRewards' own fade — both end behind cover.
            SetState(GameState.DayRewards);
            yield return ui.DayRewards.Play(CurrentDay, score, onDismissed: () =>
            {
                ui.Loading.Show();
                ui.EyeClose.Hide();
            });

            SetState(GameState.LoadingNight);
            StartCoroutine(SceneLoader.Instance.LoadAsync(SceneLoader.NightCityScene));
            yield return ui.Loading.FillBar(_minLoadingDuration);

            // NightIntro fades in over Loading → hide Loading once shown → wait click → NightIntro
            // fades out, revealing NightCity cleanly.
            yield return ui.NightIntro.Play(onShown: () => ui.Loading.Hide());

            SetState(GameState.NightCombat);
            // Zombie fight runs here. The night layer calls NightFinished() when it's over.
        }

        private IEnumerator NightFinishedRoutine()
        {
            var ui = UIManager.Instance;
            CurrentDay++;
            SetState(GameState.LoadingCity);

            // Rewards are shown BEFORE the night now (in EndDayBySleepRoutine), so the night
            // end just covers and transitions straight to the next day's intro.
            ui.Loading.Show();

            StartCoroutine(SceneLoader.Instance.LoadAsync(SceneLoader.DayCityScene));
            yield return ui.Loading.FillBar(_minLoadingDuration);

            // BeginDay's DayStart.Play handles Loading.Hide once DayStart fully covers.
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
