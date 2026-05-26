using Game.Scripts.SanityModules;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.ApartmentHunting
{
    // Sanity corruption for ApartmentHunting. Two things degrade as sanity drops:
    //  1) the X softness of the tablets' RectMask2D (default 50) grows, blurring the edges;
    //  2) scratched transparent overlays (ordered lightest -> heaviest) turn on and flicker.
    // Wire the refs + tune the per-stage arrays; the debug log fires regardless.
    public class ApartmentHuntingSanityCorruption : SanityCorruptionHandler
    {
        [Header("Tablet mask softness (X)")]
        [SerializeField] private RectMask2D[] _tabletMasks;                    // the two tablets
        [SerializeField] private int[] _softnessXByStage = { 50, 90, 140, 200 }; // Stable, Unsettled, Disturbed, Critical

        [Header("Scratch overlays (lightest -> heaviest, start disabled)")]
        [SerializeField] private GameObject[] _scratchOverlays;
        [SerializeField] private int[] _scratchCountByStage = { 0, 1, 2, 3 };   // how many flicker per stage
        [SerializeField] private Vector2 _flickerInterval = new Vector2(0.1f, 0.5f);

        private int _activeScratchCount;
        private float _flickerTimer;
        private bool _flickerOn;

        protected override void ApplyStage(SanityStage stage)
        {
            Debug.Log($"[ApartmentCorruption] stage = {stage}");
            int s = (int)stage;

            if (_tabletMasks != null && _softnessXByStage.Length > 0)
            {
                int x = _softnessXByStage[Mathf.Clamp(s, 0, _softnessXByStage.Length - 1)];
                foreach (var mask in _tabletMasks)
                    if (mask != null) mask.softness = new Vector2Int(x, mask.softness.y);
            }

            _activeScratchCount = _scratchCountByStage.Length > 0
                ? _scratchCountByStage[Mathf.Clamp(s, 0, _scratchCountByStage.Length - 1)]
                : 0;

            _flickerOn = false;
            _flickerTimer = 0f;
            ApplyScratchVisibility(false);   // hidden until the flicker turns them on
        }

        private void Update()
        {
            if (_scratchOverlays == null || _activeScratchCount <= 0) return;

            _flickerTimer -= Time.deltaTime;
            if (_flickerTimer > 0f) return;

            _flickerOn = !_flickerOn;
            ApplyScratchVisibility(_flickerOn);
            _flickerTimer = Random.Range(_flickerInterval.x, _flickerInterval.y);
        }

        private void ApplyScratchVisibility(bool on)
        {
            for (int i = 0; i < _scratchOverlays.Length; i++)
            {
                if (_scratchOverlays[i] == null) continue;
                _scratchOverlays[i].SetActive(on && i < _activeScratchCount);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _activeScratchCount = 0;
            ApplyScratchVisibility(false);
        }
    }
}
