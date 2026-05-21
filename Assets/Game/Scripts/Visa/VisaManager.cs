using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.Visa
{
    public class VisaManager : MonoBehaviour
    {
        [SerializeField] private PassportDocument _passportPrefab;
        [SerializeField] private VisaApplyDocument _visaApplyPrefab;
        [SerializeField] private AdmissionDocument _admissionPrefab;
        [SerializeField] private FundsDocument _fundsPrefab;
        [SerializeField] private LanguageDocument _languagePrefab;
        [SerializeField] private APSDocument _apsPrefab;
        [SerializeField] private InsuranceDocument _insurancePrefab;
        [SerializeField] private PhotoDocument _photoPrefab;

        [SerializeField] private Transform[] _leftPositions;
        [SerializeField] private Transform[] _rightPositions;

        [SerializeField] private float _slideDistance = 5f;
        [SerializeField] private float _slideDuration = 0.4f;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LayerMask _documentLayerMask;

        private List<MonoBehaviour> _activeDocuments = new();
        private int _currentApplicationIndex;
        private VisaApplicationData[] _applications;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 100f, _documentLayerMask))
                {
                    var clickable = hit.collider.GetComponent<IClickableDocument>();
                    clickable?.OnClick();
                }
            }
        }

        private void Start()
        {
            var json = Resources.Load<TextAsset>("Visa/applications");
            _applications = JsonUtility.FromJson<VisaApplicationListWrapper>(json.text).Items;
            LoadApplication(0);
        }

        public void LoadApplication(int index)
        {
            ClearDocuments();
            _currentApplicationIndex = index;
            var app = _applications[index];

            int leftIdx = 0;
            int rightIdx = 0;

            if (app.Passport != null)
                SpawnDocument(_passportPrefab, app.Passport, _leftPositions, ref leftIdx);

            if (app.VisaApply != null)
                SpawnDocument(_visaApplyPrefab, app.VisaApply, _rightPositions, ref rightIdx);

            if (app.Admission != null)
                SpawnDocument(_admissionPrefab, app.Admission, _leftPositions, ref leftIdx);

            if (app.Funds != null)
                SpawnDocument(_fundsPrefab, app.Funds, _rightPositions, ref rightIdx);

            if (app.Language != null)
                SpawnDocument(_languagePrefab, app.Language, _leftPositions, ref leftIdx);

            if (app.APS != null)
                SpawnDocument(_apsPrefab, app.APS, _rightPositions, ref rightIdx);

            if (app.Insurance != null)
                SpawnDocument(_insurancePrefab, app.Insurance, _leftPositions, ref leftIdx);
        }

        private void SpawnDocument<TData>(VisaDocumentBase<TData> prefab, TData data,
            Transform[] positions, ref int posIndex)
        {
            if (posIndex >= positions.Length) return;

            var target = positions[posIndex];
            posIndex++;

            var doc = Instantiate(prefab, target.position, target.rotation, transform);
            doc.Initialize(data);

            bool fromLeft = (posIndex % 2 == 0);
            Vector3 offset = (fromLeft ? Vector3.left : Vector3.right) * _slideDistance;
            doc.transform.position += offset;
            doc.transform.DOMove(target.position, _slideDuration)
                .SetEase(Ease.OutQuad)
                .SetDelay(posIndex * 0.1f)
                .OnComplete(() => doc.SetOriginalPose());

            _activeDocuments.Add(doc);
        }

        private void ClearDocuments()
        {
            foreach (var doc in _activeDocuments)
            {
                if (doc != null) Destroy(doc.gameObject);
            }
            _activeDocuments.Clear();
        }

        public void NextApplication()
        {
            if (_currentApplicationIndex + 1 >= _applications.Length) return;
            LoadApplication(_currentApplicationIndex + 1);
        }
    }
}
