using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Screens
{
    // Bootstrap-time main menu. Exposes click events that GameManager subscribes to in the
    // boot flow (slice 3). Continue is optional — null-safe if save system isn't present yet.
    public class MainMenuUI : UIScreen
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _quitButton;

        public event Action OnNewGameClicked;
        public event Action OnContinueClicked;
        public event Action OnQuitClicked;

        protected override void Awake()
        {
            base.Awake();
            if (_newGameButton  != null) _newGameButton.onClick.AddListener (() => OnNewGameClicked?.Invoke());
            if (_continueButton != null) _continueButton.onClick.AddListener(() => OnContinueClicked?.Invoke());
            if (_quitButton     != null) _quitButton.onClick.AddListener    (() => OnQuitClicked?.Invoke());
        }
    }
}
