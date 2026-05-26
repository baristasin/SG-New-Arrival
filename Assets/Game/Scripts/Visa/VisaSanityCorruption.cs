using System.Collections.Generic;
using Game.Scripts.SanityModules;
using Game.Scripts.StudentData;
using Game.Scripts.Utilities;
using UnityEngine;

namespace Game.Scripts.Visa
{
    // Sanity corruption for the Visa minigame:
    //  1) coffee stains — pooled stain sprites are scattered onto the papers, more per worse stage;
    //  2) checklist input glitches — a press can be dropped or redirected to another option.
    // VisaChecklistRow consults the static Active instance for the input glitches.
    public class VisaSanityCorruption : SanityCorruptionHandler
    {
        public static VisaSanityCorruption Active { get; private set; }

        [Header("Coffee stains")]
        [SerializeField] private CoffeeStain _stainPrefab;
        [SerializeField] private Sprite[] _stainSprites;             // ~5 transparent stain sprites
        [SerializeField] private RectTransform[] _papers;            // documents + checklist to dirty
        [SerializeField] private int[] _stainsPerPaperByStage = { 0, 1, 2, 3 };  // Stable..Critical
        [SerializeField] private int _poolSize = 36;
        [SerializeField] private Vector2 _stainScaleRange = new Vector2(0.6f, 1.2f);

        [Header("Checklist input glitch (chance per stage)")]
        [SerializeField] private float[] _dropChanceByStage = { 0f, 0.1f, 0.2f, 0.35f };   // press ignored
        [SerializeField] private float[] _flipChanceByStage = { 0f, 0.1f, 0.2f, 0.35f };   // press hits another option

        private Pool<CoffeeStain> _pool;
        private readonly List<CoffeeStain> _activeStains = new();

        protected override void OnEnable()
        {
            Active = this;
            if (_pool == null && _stainPrefab != null)
                _pool = new Pool<CoffeeStain>(_stainPrefab, _poolSize);
            base.OnEnable();   // subscribes + applies current stage
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ClearStains();
            if (Active == this) Active = null;
        }

        protected override void ApplyStage(SanityStage stage)
        {
            Debug.Log($"[VisaCorruption] stage = {stage}");
            ClearStains();

            if (_pool == null || _papers == null || _stainSprites == null || _stainSprites.Length == 0) return;

            int count = _stainsPerPaperByStage.Length > 0
                ? _stainsPerPaperByStage[Mathf.Clamp((int)stage, 0, _stainsPerPaperByStage.Length - 1)]
                : 0;
            if (count <= 0) return;

            foreach (var paper in _papers)
            {
                if (paper == null) continue;
                Vector2 size = paper.rect.size;

                for (int i = 0; i < count; i++)
                {
                    var stain = _pool.Get();
                    var pos = new Vector2(
                        Random.Range(-size.x * 0.5f, size.x * 0.5f),
                        Random.Range(-size.y * 0.5f, size.y * 0.5f));
                    stain.Show(
                        _stainSprites[Random.Range(0, _stainSprites.Length)],
                        paper, pos,
                        Random.Range(0f, 360f),
                        Random.Range(_stainScaleRange.x, _stainScaleRange.y));
                    _activeStains.Add(stain);
                }
            }
        }

        private void ClearStains()
        {
            if (_pool == null) { _activeStains.Clear(); return; }

            foreach (var stain in _activeStains)
            {
                if (stain == null) continue;
                stain.transform.SetParent(transform, false);
                _pool.ReturnToPool(stain);
            }
            _activeStains.Clear();
        }

        // Called by VisaChecklistRow on a button press. Returns the status to apply,
        // or null if the press should be dropped (ignored).
        public CheckStatus? FilterInput(CheckStatus requested)
        {
            int s = Sanity != null ? Mathf.Clamp((int)Sanity.Stage, 0, 3) : 0;

            if (s < _dropChanceByStage.Length && Random.value < _dropChanceByStage[s])
                return null;

            if (s < _flipChanceByStage.Length && Random.value < _flipChanceByStage[s])
                return RandomOther(requested);

            return requested;
        }

        private static CheckStatus RandomOther(CheckStatus requested)
        {
            CheckStatus other;
            do { other = (CheckStatus)Random.Range(0, 3); } while (other == requested);
            return other;
        }
    }
}
