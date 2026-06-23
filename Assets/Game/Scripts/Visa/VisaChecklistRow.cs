using Game.Scripts.StudentData;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Visa
{
    // One requirement row: three exclusive option buttons (Yes / No / NotRequired),
    // each with a tick GameObject shown when that option is the current selection.
    public class VisaChecklistRow : MonoBehaviour
    {
        [SerializeField] private VisaRequirement _requirement;

        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        [SerializeField] private Button _notRequiredButton;

        [SerializeField] private GameObject _yesTick;
        [SerializeField] private GameObject _noTick;
        [SerializeField] private GameObject _notRequiredTick;

        public VisaRequirement Requirement => _requirement;
        public bool IsAnswered => _selected.HasValue;
        public CheckStatus? Selected => _selected;

        private CheckStatus? _selected;

        private void Awake()
        {
            if (_yesButton != null) _yesButton.onClick.AddListener(() => Press(CheckStatus.Yes));
            if (_noButton != null) _noButton.onClick.AddListener(() => Press(CheckStatus.No));
            if (_notRequiredButton != null) _notRequiredButton.onClick.AddListener(() => Press(CheckStatus.NotRequired));
            Clear();
        }

        // Player input goes through the sanity corruption (if active): it may be dropped or
        // redirected to a different option as sanity drops.
        private void Press(CheckStatus requested)
        {
            var corruption = VisaSanityCorruption.Active;
            if (corruption == null)
            {
                Select(requested);
                return;
            }

            var result = corruption.FilterInput(requested);
            if (result.HasValue)
                Select(result.Value);
            // else: the press was dropped — nothing happens
        }

        public void Clear()
        {
            _selected = null;
            RefreshTicks();
        }

        private void Select(CheckStatus status)
        {
            _selected = status;
            RefreshTicks();
            VisaManager.Active?.PlayPenWrite();
        }

        private void RefreshTicks()
        {
            if (_yesTick != null) _yesTick.SetActive(_selected == CheckStatus.Yes);
            if (_noTick != null) _noTick.SetActive(_selected == CheckStatus.No);
            if (_notRequiredTick != null) _notRequiredTick.SetActive(_selected == CheckStatus.NotRequired);
        }
    }
}
