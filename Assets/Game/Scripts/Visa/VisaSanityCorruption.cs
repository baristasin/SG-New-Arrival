using System.Collections.Generic;
using Game.Scripts.SanityModules;
using Game.Scripts.StudentData;
using UnityEngine;

namespace Game.Scripts.Visa
{
    // Sanity corruption for the Visa minigame:
    //  1) coffee-stain prefabs are instantiated onto the papers, ADDITIVELY — existing stains stay
    //     and only the delta (new total - current) is added; new stains pick prefabs that aren't
    //     already on that paper, so you don't get exact-position duplicates. On a recovery to a
    //     lower stage, extras are removed from the top (most-recent first), keeping the oldest;
    //  2) checklist input glitches — a press can be dropped or redirected to another option.
    // VisaChecklistRow consults the static Active instance for the input glitches.
    public class VisaSanityCorruption : SanityCorruptionHandler
    {
        public static VisaSanityCorruption Active { get; private set; }

        [Header("Coffee stains")]
        [SerializeField] private List<GameObject> _stainPrefabs;          // pre-styled stain sheets
        [SerializeField] private RectTransform[] _papers;                 // documents + checklist to dirty
        [SerializeField] private int[] _stainsPerPaperByStage = { 0, 1, 2, 3 };

        [Header("Checklist input glitch (chance per stage)")]
        [SerializeField] private float[] _dropChanceByStage = { 0f, 0.1f, 0.2f, 0.35f };
        [SerializeField] private float[] _flipChanceByStage = { 0f, 0.1f, 0.2f, 0.35f };

        // Per-paper state: parallel lists of spawned instances + which prefab index each used.
        private class PaperStains
        {
            public readonly List<GameObject> Instances = new();
            public readonly List<int> PrefabIndices = new();
            public int Count => Instances.Count;
        }
        private readonly Dictionary<RectTransform, PaperStains> _stains = new();
        private readonly List<int> _shuffleBuffer = new();

        protected override void OnEnable()
        {
            Active = this;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ClearAllStains();
            if (Active == this) Active = null;
        }

        protected override void ApplyStage(SanityStage stage)
        {
            Debug.Log($"[VisaCorruption] stage = {stage}");

            if (_papers == null || _stainPrefabs == null || _stainPrefabs.Count == 0) return;

            int target = _stainsPerPaperByStage.Length > 0
                ? _stainsPerPaperByStage[Mathf.Clamp((int)stage, 0, _stainsPerPaperByStage.Length - 1)]
                : 0;

            foreach (var paper in _papers)
            {
                if (paper == null) continue;
                SyncStainsOn(paper, target);
            }
        }

        // Bring this paper's stain count to `target`. Existing stains are preserved; only the delta
        // is added (or, if target dropped, trimmed from the end). New stains avoid prefab indices
        // already used on the paper so we don't stack identical sheets.
        private void SyncStainsOn(RectTransform paper, int target)
        {
            if (!_stains.TryGetValue(paper, out var state))
            {
                state = new PaperStains();
                _stains[paper] = state;
            }

            // Trim from the most recent if we have too many (e.g. sanity recovered).
            while (state.Count > target)
            {
                int last = state.Count - 1;
                if (state.Instances[last] != null) Destroy(state.Instances[last]);
                state.Instances.RemoveAt(last);
                state.PrefabIndices.RemoveAt(last);
            }

            int delta = target - state.Count;
            if (delta <= 0) return;

            // Prefab indices not yet used on this paper, shuffled.
            _shuffleBuffer.Clear();
            for (int i = 0; i < _stainPrefabs.Count; i++)
                if (!state.PrefabIndices.Contains(i)) _shuffleBuffer.Add(i);

            for (int i = _shuffleBuffer.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (_shuffleBuffer[i], _shuffleBuffer[j]) = (_shuffleBuffer[j], _shuffleBuffer[i]);
            }

            int fromUnused = Mathf.Min(delta, _shuffleBuffer.Count);
            for (int k = 0; k < fromUnused; k++)
                SpawnStain(state, paper, _shuffleBuffer[k]);

            // Target may exceed the number of distinct prefabs — fill the rest with random repeats.
            int stillNeeded = delta - fromUnused;
            while (stillNeeded > 0)
            {
                int idx = Random.Range(0, _stainPrefabs.Count);
                SpawnStain(state, paper, idx);
                stillNeeded--;
            }
        }

        private void SpawnStain(PaperStains state, RectTransform paper, int prefabIdx)
        {
            var prefab = _stainPrefabs[prefabIdx];
            if (prefab == null) return;
            var instance = Instantiate(prefab, paper, false);
            state.Instances.Add(instance);
            state.PrefabIndices.Add(prefabIdx);
        }

        private void ClearAllStains()
        {
            foreach (var kv in _stains)
            {
                foreach (var inst in kv.Value.Instances)
                    if (inst != null) Destroy(inst);
                kv.Value.Instances.Clear();
                kv.Value.PrefabIndices.Clear();
            }
            _stains.Clear();
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
