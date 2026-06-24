using Game.Scripts.SanityModules;
using Game.Scripts.UI.Screens;
using Game.Scripts.Utilities;
using UnityEngine;

namespace Game.Scripts.UI
{
    // Persistent, global UI hub. Owns the cross-scene Canvas and hosts every UIScreen — main
    // menu, loading overlay, day intro, day HUD, tutorial pages, sleep transition, day rewards,
    // and the night combat HUD. Other systems (GameManager, minigames) reach screens through
    // the typed properties below.
    public class UIManager : PersistentSingleton<UIManager>
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private SanityBar _sanityBar;

        [SerializeField] private MainMenuUI _mainMenu;
        [SerializeField] private LoadingScreenUI _loading;
        [SerializeField] private DayStartUI _dayStart;
        [SerializeField] private DayHUD _dayHUD;
        [SerializeField] private TutorialUI _tutorial;
        [SerializeField] private EyeCloseUI _eyeClose;
        [SerializeField] private DayRewardsUI _dayRewards;
        [SerializeField] private NightIntroUI _nightIntro;
        [SerializeField] private NightResultUI _nightResult;

        public Canvas Canvas => _canvas;
        public SanityBar SanityBar => _sanityBar;

        public MainMenuUI MainMenu => _mainMenu;
        public LoadingScreenUI Loading => _loading;
        public DayStartUI DayStart => _dayStart;
        public DayHUD DayHUD => _dayHUD;
        public TutorialUI Tutorial => _tutorial;
        public EyeCloseUI EyeClose => _eyeClose;
        public DayRewardsUI DayRewards => _dayRewards;
        public NightIntroUI NightIntro => _nightIntro;
        public NightResultUI NightResult => _nightResult;
    }
}
