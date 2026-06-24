using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Game.Scripts.StudentData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Visa
{
    public class VisaManager : MonoBehaviour, IMinigameManager
    {
        public static VisaManager Active { get; private set; }

        [SerializeField] private EventReference _paperSlideEvent;
        [SerializeField] private EventReference _penWriteEvent;
        [SerializeField] private EventReference _submitEvent;
        [SerializeField] private EventReference _stainEvent;

        [SerializeField] private VisaChecklist _checklist;

        [SerializeField] private UnityEngine.UI.Button _submitButton;
        [SerializeField] private float _submitCooldown = 4f;

        public int GetScorePercent()
        {
            if (_checklist == null)
            {
                Debug.LogWarning("[VisaManager] _checklist not wired — score will always be 0.");
                return 0;
            }
            return _checklist.TotalScore;   // raw points: 2 per correct checkbox
        }

        private void Awake() { Active = this; }
        private void OnDestroy() { if (Active == this) Active = null; }

        public void PlayPaperSlide() { if (!_paperSlideEvent.IsNull) AudioManager.Instance.PlayOneShot(_paperSlideEvent); }
        public void PlayPenWrite()   { if (!_penWriteEvent.IsNull)   AudioManager.Instance.PlayOneShot(_penWriteEvent); }
        public void PlaySubmit()     { if (!_submitEvent.IsNull)     AudioManager.Instance.PlayOneShot(_submitEvent); }
        public void PlayStain()      { if (!_stainEvent.IsNull)      AudioManager.Instance.PlayOneShot(_stainEvent); }

        [SerializeField] private StudentDatabase _studentDatabase;

        [SerializeField] private PassportDocument _passportDoc;
        [SerializeField] private VisaApplyDocument _visaApplyDoc;
        [SerializeField] private AdmissionDocument _admissionDoc;
        [SerializeField] private FundsDocument _fundsDoc;
        [SerializeField] private LanguageDocument _languageDoc;
        [SerializeField] private APSDocument _apsDoc;
        [SerializeField] private InsuranceDocument _insuranceDoc;
        [SerializeField] private PhotoDocument _photoDoc;

        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LayerMask _documentLayerMask;

        [SerializeField] private List<Sprite> _studentPhotos;

        // Optional X/close UI button shown only while a document is open. Wire its
        // Button.onClick to CloseOpenDocument(); the manager toggles its visibility.
        [SerializeField] private GameObject _closeButton;

        [SerializeField] private float _stagger = 0.08f;

        private IClickableDocument _openDocument;
        private IClickableDocument _hoveredDocument;
        private int _studentIndex;
        private int _completedRounds;   // sequential for the first pass, then random
        private bool _isTransitioning;

        // Raised after a student's documents (re)load, and before the current ones leave, so
        // UI such as the checklist can slide in / out and reset itself.
        public event System.Action StudentLoaded;
        public event System.Action StudentUnloading;

        public StudentProfile CurrentStudent =>
            _studentDatabase != null && _studentIndex < _studentDatabase.Students.Count
                ? _studentDatabase.Students[_studentIndex]
                : null;

        // Called by MinigameStation after the tutorial closes. Loads the first student and plays
        // the entrance slide-in (papers travel from their slideAnchor to their placed positions).
        public void BeginGame()
        {
            SetCloseButtonVisible(false);
            _completedRounds = 0;
            if (_checklist != null) _checklist.ResetScore();
            LoadStudent(0);
        }

        [Button]
        public void LoadStudent(int index)
        {
            if (_studentDatabase == null || _studentDatabase.Students.Count == 0)
            {
                Debug.LogWarning("[VisaManager] No StudentDatabase assigned (or it is empty).");
                return;
            }

            ResetInteraction();
            _studentIndex = Mathf.Clamp(index, 0, _studentDatabase.Students.Count - 1);
            PopulateDocuments(_studentDatabase.Students[_studentIndex]);
            PlayEntrance();
            StartCoroutine(LockSubmit());
            StudentLoaded?.Invoke();
        }

        // Disables the End/Submit button for _submitCooldown seconds so the player can't spam
        // through students before papers have slid into place.
        private IEnumerator LockSubmit()
        {
            if (_submitButton == null) yield break;
            _submitButton.interactable = false;
            yield return new WaitForSeconds(_submitCooldown);
            _submitButton.interactable = true;
        }

        // Wire the "next / approve" button here. Current papers slide out, then the next
        // student's papers slide in. Sequential for the first pass through the database, then
        // every subsequent pick is random — sanity decides when the day ends.
        public void NextStudent()
        {
            if (_isTransitioning || _studentDatabase == null || _studentDatabase.Students.Count == 0)
                return;

            // Lock the button the INSTANT the player clicks — spam clicks during the slide-out
            // transition must not register.
            StartCoroutine(LockSubmit());

            _completedRounds++;
            int count = _studentDatabase.Students.Count;
            int next = _completedRounds < count ? _completedRounds : Random.Range(0, count);
            StartCoroutine(TransitionTo(next));
        }

        private IEnumerator TransitionTo(int nextIndex)
        {
            _isTransitioning = true;

            _openDocument = null;
            _hoveredDocument = null;
            SetCloseButtonVisible(false);

            StudentUnloading?.Invoke();
            float wait = PlayExit();
            yield return new WaitForSeconds(wait);

            _isTransitioning = false;
            LoadStudent(nextIndex);
        }

        private void PlayEntrance()
        {
            int i = 0;
            foreach (var doc in Documents())
            {
                if (!doc.IsActive) continue;
                doc.SlideIn(i * _stagger);
                i++;
            }
            PlayPaperSlide();   // single shot for the whole batch
        }

        // Place active docs directly at their home pose with no animation (used on first load).
        private void SnapAllHome()
        {
            foreach (var doc in Documents())
            {
                if (!doc.IsActive) continue;
                doc.SnapHome();
            }
        }

        private float PlayExit()
        {
            float maxEnd = 0f;
            int i = 0;
            foreach (var doc in Documents())
            {
                if (!doc.IsActive) continue;
                float delay = i * _stagger;
                doc.SlideOut(delay);
                maxEnd = Mathf.Max(maxEnd, delay + doc.SlideOutDuration);
                i++;
            }
            return maxEnd;
        }

        private IEnumerable<IClickableDocument> Documents()
        {
            if (_passportDoc != null) yield return _passportDoc;
            if (_visaApplyDoc != null) yield return _visaApplyDoc;
            if (_admissionDoc != null) yield return _admissionDoc;
            if (_fundsDoc != null) yield return _fundsDoc;
            if (_languageDoc != null) yield return _languageDoc;
            if (_apsDoc != null) yield return _apsDoc;
            if (_insuranceDoc != null) yield return _insuranceDoc;
            if (_photoDoc != null) yield return _photoDoc;
        }

        // Clears any open/hovered document — used when switching to a new student.
        private void ResetInteraction()
        {
            if (_hoveredDocument != null)
            {
                _hoveredDocument.SetHighlight(false);
                _hoveredDocument = null;
            }
            if (_openDocument != null)
            {
                _openDocument.Close();
                _openDocument = null;
            }
            SetCloseButtonVisible(false);
        }

        // Identity comes from the StudentProfile, visa-specific fields from StudentProfile.Visa.
        // Documents left unassigned in the inspector are simply skipped.
        private void PopulateDocuments(StudentProfile ssudent)
        {
            var visaData = ssudent.Visa;
            if (visaData == null)
            {
                Debug.LogWarning($"[VisaManager] {ssudent.FullName} has no Visa data.");
                return;
            }

            // Passport & Admission are always part of the application.
            if (_passportDoc != null)
            {
                _passportDoc.gameObject.SetActive(true);
                _passportDoc.Initialize(new PassportData
                {
                    Id = visaData.PassportId,
                    Name = ssudent.FullName,
                    Nationality = ssudent.Nationality,
                    ExpiresAt = visaData.PassportExpiryDate,
                    IssuedAt = visaData.PassportIssued,
                });
            }

            if (_admissionDoc != null)
            {
                bool has = visaData.AdmissionDeadline != "";
                _admissionDoc.gameObject.SetActive(has);
                _admissionDoc.Initialize(new AdmissionData
                {
                    Name = ssudent.FullName,
                    AdmissionDeadlineDate = visaData.AdmissionDeadline,
                });
            }

            // The rest only appear when the student actually has that data; otherwise the
            // document GameObject is deactivated so the player never sees that paper.
            if (_fundsDoc != null)
            {
                bool has = visaData.FinancialFunds > 0;
                _fundsDoc.gameObject.SetActive(has);
                if (has)
                    _fundsDoc.Initialize(new FundsData
                    {
                        FirstName = ssudent.FirstName,
                        LastName = ssudent.LastName,
                        DateOfBirth = ssudent.DateOfBirth,
                        PassportNumber = visaData.PassportId,
                        IbanNumber = visaData.Iban,
                        BlockedAccountAmount = visaData.FinancialFunds,
                    });
            }

            if (_languageDoc != null)
            {
                bool has = visaData.LanguageCertLevel != LanguageLevel.None;
                _languageDoc.gameObject.SetActive(has);
                if (has)
                    _languageDoc.Initialize(new LanguageData
                    {
                        Provider = visaData.LanguageCertProvider,
                        Date = visaData.LanguageCertDate,
                        Name = ssudent.FullName,
                        Level = visaData.LanguageCertLevel,
                    });
            }

            if (_apsDoc != null)
            {
                bool has = !string.IsNullOrEmpty(visaData.ApsName);
                _apsDoc.gameObject.SetActive(has);
                if (has)
                    _apsDoc.Initialize(new APSData { Name = visaData.ApsName });
            }

            if (_insuranceDoc != null)
            {
                bool has = !string.IsNullOrEmpty(visaData.TravelInsurancePerson);
                _insuranceDoc.gameObject.SetActive(has);
                if (has)
                    _insuranceDoc.Initialize(new InsuranceData
                    {
                        IssuedPerson = visaData.TravelInsurancePerson,
                        DateOfBirth = ssudent.DateOfBirth,
                    });
            }

            if (_visaApplyDoc != null)
            {
                bool has = visaData.VisaPaymentOrder > 0f;
                _visaApplyDoc.gameObject.SetActive(has);
                if (has)
                    _visaApplyDoc.Initialize(new VisaApplyData { PayAmount = visaData.VisaPaymentOrder});
            }

            if (_photoDoc != null)
            {
                bool has = visaData.PhotosTotal > 0;
                _photoDoc.gameObject.SetActive(has);
                if (has)
                    _photoDoc.Initialize(new PhotoData
                    {
                        PhotoCount = visaData.PhotosTotal,
                        Photo = GetStudentPhoto(ssudent),
                        Quality = visaData.PhotosQuality,
                    });
            }
        }

        // One biometric photo sprite per student, indexed by IdNumber (element 0 = IdNumber 1).
        // PhotoDocument shows that same sprite PhotoCount (2–3) times.
        private Sprite GetStudentPhoto(StudentProfile student)
        {
            if (_studentPhotos == null) return null;
            int index = student.IdNumber - 1;
            return index >= 0 && index < _studentPhotos.Count ? _studentPhotos[index] : null;
        }

        private void Update()
        {
            IClickableDocument under = RaycastDocument();
            UpdateHover(under);

            if (Input.GetMouseButtonDown(0) && under != null)
                HandleClick(under);

            if (Input.GetKeyDown(KeyCode.Escape))
                CloseOpenDocument();
        }

        private IClickableDocument RaycastDocument()
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100f, _documentLayerMask))
                return hit.collider.GetComponent<IClickableDocument>();
            return null;
        }

        private void UpdateHover(IClickableDocument under)
        {
            // The focused (open) document is never highlighted.
            if (ReferenceEquals(under, _openDocument))
                under = null;

            if (ReferenceEquals(under, _hoveredDocument)) return;

            _hoveredDocument?.SetHighlight(false);
            _hoveredDocument = under;
            _hoveredDocument?.SetHighlight(true);
        }

        private void HandleClick(IClickableDocument doc)
        {
            if (doc.IsAnimating) return;

            // Clicking the already-open document closes it.
            if (ReferenceEquals(doc, _openDocument))
            {
                CloseOpenDocument();
                return;
            }

            // Only one document is focused at a time — close the previous one first.
            _openDocument?.Close();

            doc.SetHighlight(false);
            if (ReferenceEquals(doc, _hoveredDocument))
                _hoveredDocument = null;

            doc.Open();
            _openDocument = doc;
            SetCloseButtonVisible(true);
        }

        // Public so an X/close UI button can call it via Button.onClick. Also fired by ESC.
        public void CloseOpenDocument()
        {
            if (_openDocument == null) return;

            _openDocument.Close();
            _openDocument = null;
            SetCloseButtonVisible(false);
        }

        private void SetCloseButtonVisible(bool visible)
        {
            if (_closeButton != null)
                _closeButton.SetActive(visible);
        }
    }
}
