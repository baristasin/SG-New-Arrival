using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Visa
{
    // Holds the 8 requirement rows, slides itself in/out with the round, resets the rows on a
    // new student, and scores the player's selections against the current student's answer key.
    public class VisaChecklist : MonoBehaviour
    {
        [SerializeField] private VisaManager _visaManager;
        [SerializeField] private VisaChecklistRow[] _rows;
        [SerializeField] private TextMeshProUGUI _resultText;

        [SerializeField] private float _slideOffsetX = -60f;
        [SerializeField] private float _slideDuration = 0.5f;

        public int CorrectCount { get; private set; }
        public int TotalScore { get; private set; }
        public int Total => _rows != null ? _rows.Length : 0;

        private RectTransform _rect;
        private Vector2 _home;
        private bool _roundEnded;

        private void Awake()
        {
            _rect = transform as RectTransform;
            if (_rect == null) return;

            // Capture authored home pos first, then snap off-screen so the checklist is HIDDEN
            // while the tutorial is up. OnStudentLoaded → SlideIn tweens it back from here.
            _home = _rect.anchoredPosition;
            _rect.anchoredPosition = _home + new Vector2(_slideOffsetX, 0f);
        }

        private void OnEnable()
        {
            if (_visaManager == null) return;
            _visaManager.StudentLoaded += OnStudentLoaded;
            _visaManager.StudentUnloading += SlideOut;
        }

        private void OnDisable()
        {
            if (_visaManager == null) return;
            _visaManager.StudentLoaded -= OnStudentLoaded;
            _visaManager.StudentUnloading -= SlideOut;
        }

        private void OnStudentLoaded()
        {
            _roundEnded = false;
            ResetSelections();
            SlideIn();
        }

        public void SlideIn()
        {
            if (_rect == null) return;
            _rect.DOKill();
            _rect.anchoredPosition = _home + new Vector2(_slideOffsetX, 0f);
            _rect.DOAnchorPos(_home, _slideDuration).SetEase(Ease.OutQuad);
        }

        public void SlideOut()
        {
            if (_rect == null) return;
            _rect.DOKill();
            _rect.DOAnchorPos(_home + new Vector2(_slideOffsetX, 0f), _slideDuration).SetEase(Ease.InQuad);
        }

        public void ResetSelections()
        {
            if (_rows != null)
                foreach (var row in _rows)
                    if (row != null) row.Clear();

            if (_resultText != null) _resultText.text = string.Empty;
        }

        // Called by VisaManager at BeginGame to wipe the cumulative score for a fresh run.
        public void ResetScore()
        {
            TotalScore = 0;
            CorrectCount = 0;
        }

        // Wire a Submit/Approve button to this. Score = options matching the answer key.
        public void Submit()
        {
            var student = _visaManager != null ? _visaManager.CurrentStudent : null;
            if (student == null || student.Visa == null)
            {
                Debug.LogWarning("[VisaChecklist] No current student to evaluate against.");
                return;
            }

            CorrectCount = 0;
            foreach (var row in _rows)
            {
                if (row == null || !row.IsAnswered) continue;
                if (row.Selected == VisaRules.Expected(student.Visa, row.Requirement))
                    CorrectCount++;
            }

            if (_resultText != null) _resultText.text = $"{CorrectCount} / {Total}";
        }

        // Wire the End button here: scores the round once, adds it to the running total,
        // then tells the manager to bring the next student.
        public void EndRound()
        {
            if (_roundEnded) return;
            _roundEnded = true;

            Submit();
            TotalScore += CorrectCount * 2;   // each correct checkbox = 2 points

            VisaManager.Active?.PlaySubmit();
            if (_visaManager != null) _visaManager.NextStudent();
        }
    }
}
