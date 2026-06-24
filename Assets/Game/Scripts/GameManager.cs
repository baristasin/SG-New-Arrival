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
        public MinigameId? CurrentMinigameId { get; private set; }

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


        public void RegisterCity(CityHub hub)   { CurrentCity = hub; }
        public void UnregisterCity(CityHub hub) { if (CurrentCity == hub) CurrentCity = null; }


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
            MusicController.Instance?.PlayDay();
        }

        private void WireSanity()
        {
            var sanity = SanityManager.Instance;
            if (sanity == null) return;
            sanity.OnSanityChanged -= HandleSanityChanged;
            sanity.OnSanityChanged += HandleSanityChanged;
        }
        
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

        private IEnumerator DevSkipToNightRoutine()
        {
            CurrentDay = Mathf.Max(1, _devSkipDay);
            SetState(GameState.LoadingNight);

            var ui = UIManager.Instance;
            ui.Loading.Show();
            yield return ui.MainMenu.Hide().WaitForCompletion();

            MusicController.Instance?.StopAll();

            StartCoroutine(SceneLoader.Instance.LoadAsync(SceneLoader.NightCityScene));
            yield return ui.Loading.FillBar(_minLoadingDuration);

            yield return ui.NightIntro.Play(onShown: () => ui.Loading.Hide());


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
            CurrentMinigameId = id;
            StartCoroutine(EnterMinigameRoutine(id));
        }
        
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
            int i = ((CurrentDay - 1) % _dayMinigames.Count + _dayMinigames.Count) % _dayMinigames.Count;
            return _dayMinigames[i];
        }


        [Button]
        public void NightFinished()
        {
            if (State != GameState.NightCombat) return;
            StartCoroutine(NightFinishedRoutine());
        }


        private IEnumerator NewGameRoutine()
        {
            CurrentDay = 1;
            SetState(GameState.LoadingCity);

            var ui = UIManager.Instance;


            ui.Loading.Show();
            yield return ui.MainMenu.Hide().WaitForCompletion();


            StartCoroutine(SceneLoader.Instance.LoadAsync(SceneLoader.DayCityScene));
            yield return ui.Loading.FillBar(_newGameLoadingDuration);

            yield return BeginDay();
        }

        private IEnumerator BeginDay()
        {
            SetState(GameState.DayStart);

            var ui = UIManager.Instance;
            var sanity = SanityManager.Instance;

            sanity.ResetSanity();
            sanity.DrainPerSecond = 0f;          
            Clock.Reset(_dayStartHour);

            Clock.OnHourPassed -= HandleHourPassedCity;
            Clock.OnHourPassed += HandleHourPassedCity;


            yield return ui.DayStart.Play(CurrentDay, onShown: () => ui.Loading.Hide());

            ui.DayHUD.Show();
            Clock.Resume();

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

            ui.Loading.Show();
            ui.DayHUD.Hide();   // fire-and-forget; fades behind Loading
            CurrentCity.EnterStation(id);
            yield return ui.Loading.FillBar(_minLoadingDuration);

            SetState(GameState.Tutorial);
            yield return ui.Tutorial.Play(id, onShown: () => ui.Loading.Hide());


            CurrentCity.GetStation(id)?.BeginGame();


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
            CurrentMinigameId = null;

            MusicController.Instance?.StopAll();

            var ui = UIManager.Instance;
            ui.DayHUD.Hide();   

            yield return ui.EyeClose.Play();

            int score = 0;
            var todayId = GetTodayMinigame();
            var station = (todayId.HasValue && CurrentCity != null) ? CurrentCity.GetStation(todayId.Value) : null;
            if (station != null) score = station.GetScorePercent();
            Debug.Log($"[GameManager] Sleep score lookup: todayId={todayId} city={CurrentCity} station={station} score={score}");
            
            SetState(GameState.DayRewards);
            yield return ui.DayRewards.Play(CurrentDay, score, onDismissed: () =>
            {
                ui.Loading.Show();
                ui.EyeClose.Hide();
            });

            SetState(GameState.LoadingNight);
            StartCoroutine(SceneLoader.Instance.LoadAsync(SceneLoader.NightCityScene));
            yield return ui.Loading.FillBar(_minLoadingDuration);

  
            yield return ui.NightIntro.Play(onShown: () => ui.Loading.Hide());

            SetState(GameState.NightCombat);
        }

        private IEnumerator NightFinishedRoutine()
        {
            var ui = UIManager.Instance;
            CurrentDay++;
            SetState(GameState.LoadingCity);
            
            ui.Loading.Show();

            StartCoroutine(SceneLoader.Instance.LoadAsync(SceneLoader.DayCityScene));
            yield return ui.Loading.FillBar(_minLoadingDuration);

            yield return BeginDay();
        }
        
        private void HandleHourPassedCity(int hour)
        {
            if (State != GameState.CityRoaming) return;
            if (hour < _missionDeadlineHour) return;

            var balance = BalanceVariables.Instance;
            if (balance != null)
                SanityManager.Instance.DrainPerSecond = balance.LateDrainPerSecond;
        }


        protected void SetState(GameState next)
        {
            if (State == next) return;
            var prev = State;
            State = next;
            OnStateChanged?.Invoke(prev, next);
        }
    }
}
