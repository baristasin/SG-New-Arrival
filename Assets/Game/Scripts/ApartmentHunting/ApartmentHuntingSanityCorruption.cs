using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Game.Scripts.SanityModules;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.ApartmentHunting
{
    // Sanity corruption for ApartmentHunting (two screens). Three things degrade as sanity drops:
    //  1) the X softness of the tablets' RectMask2D (default 50) grows;
    //  2) "broken" prefabs are instantiated under each screen's broken-parent — same process per
    //     screen, but a screen can be marked Mirror so its brokens are X-flipped (visual variety);
    //  3) at Critical only, a full-screen "TV static" prefab pulses on each screen in sync:
    //     visible 2s → fade to 0 over 0.5s → wait 7-8s → repeat. CanvasGroup (preferred) or Image.
    public class ApartmentHuntingSanityCorruption : SanityCorruptionHandler
    {
        [Serializable]
        public class Screen
        {
            public RectTransform BrokenParent;
            public RectTransform TvStaticParent;
            public bool MirrorBrokens;
        }

        [SerializeField] private RectMask2D[] _tabletMasks;
        [SerializeField] private int[] _softnessXByStage = { 50, 90, 140, 200 };

        [SerializeField] private Screen[] _screens;

        [SerializeField] private List<GameObject> _brokenPrefabs;
        [SerializeField] private int[] _brokenCountByStage = { 0, 1, 2, 4 };

        [SerializeField] private GameObject _tvStaticPrefab;
        [SerializeField] private float _tvStaticVisibleTime = 2f;
        [SerializeField] private float _tvStaticFadeTime = 0.5f;
        [SerializeField] private Vector2 _tvStaticIntervalRange = new Vector2(7f, 8f);

        private readonly List<GameObject> _activeBrokens = new();
        private readonly List<GameObject> _tvStaticInstances = new();
        private Coroutine _tvStaticCoroutine;
        private EventInstance _tvBuzzInstance;

        protected override void ApplyStage(SanityStage stage)
        {
            int s = (int)stage;

            // 1) tablet mask softness
            if (_tabletMasks != null && _softnessXByStage.Length > 0)
            {
                int x = _softnessXByStage[Mathf.Clamp(s, 0, _softnessXByStage.Length - 1)];
                foreach (var mask in _tabletMasks)
                    if (mask != null) mask.softness = new Vector2Int(x, mask.softness.y);
            }

            // 2) brokens on each screen (sequential: first N from the list)
            ClearBrokens();
            int count = _brokenCountByStage.Length > 0
                ? _brokenCountByStage[Mathf.Clamp(s, 0, _brokenCountByStage.Length - 1)]
                : 0;
            if (count > 0 && _brokenPrefabs != null && _brokenPrefabs.Count > 0 && _screens != null)
            {
                int useCount = Mathf.Min(count, _brokenPrefabs.Count);
                foreach (var screen in _screens)
                {
                    if (screen == null || screen.BrokenParent == null) continue;
                    for (int i = 0; i < useCount; i++)
                    {
                        var prefab = _brokenPrefabs[i];
                        if (prefab == null) continue;
                        var instance = Instantiate(prefab, screen.BrokenParent, false);
                        if (screen.MirrorBrokens) ApplyMirror(instance);
                        _activeBrokens.Add(instance);
                    }
                }
            }

            // 3) TV static — Critical only, in sync across screens
            if (stage == SanityStage.Critical) StartTvStatic();
            else StopTvStatic();
        }

        private static void ApplyMirror(GameObject instance)
        {
            var rt = instance.transform as RectTransform;
            if (rt == null) return;

            var ap = rt.anchoredPosition;
            ap.x = -ap.x;
            rt.anchoredPosition = ap;

            var s = rt.localScale;
            s.x = -s.x;
            rt.localScale = s;
        }

        private void ClearBrokens()
        {
            foreach (var b in _activeBrokens)
                if (b != null) Destroy(b);
            _activeBrokens.Clear();
        }

        private void StartTvStatic()
        {
            if (_tvStaticPrefab == null || _screens == null) return;
            if (_tvStaticInstances.Count > 0) return;   // already running

            foreach (var screen in _screens)
            {
                if (screen == null || screen.TvStaticParent == null) continue;
                _tvStaticInstances.Add(Instantiate(_tvStaticPrefab, screen.TvStaticParent, false));
            }

            if (_tvStaticInstances.Count > 0)
                _tvStaticCoroutine = StartCoroutine(TvStaticLoop());

            // Audio: start the buzzing loop alongside the visual static.
            var mgr = ApartmentHuntingManager.Active;
            if (mgr != null && !mgr.TvBuzzingEvent.IsNull && !_tvBuzzInstance.isValid())
                _tvBuzzInstance = AudioManager.Instance.PlayLoop(mgr.TvBuzzingEvent);
        }

        private void StopTvStatic()
        {
            if (_tvStaticCoroutine != null)
            {
                StopCoroutine(_tvStaticCoroutine);
                _tvStaticCoroutine = null;
            }
            foreach (var inst in _tvStaticInstances)
                if (inst != null) Destroy(inst);
            _tvStaticInstances.Clear();

            if (_tvBuzzInstance.isValid())
                AudioManager.Instance.Stop(ref _tvBuzzInstance);
        }

        private IEnumerator TvStaticLoop()
        {
            while (true)
            {
                SetAlphaAll(1f);
                yield return new WaitForSeconds(_tvStaticVisibleTime);

                float t = 0f;
                while (t < _tvStaticFadeTime)
                {
                    t += Time.deltaTime;
                    SetAlphaAll(1f - Mathf.Clamp01(t / _tvStaticFadeTime));
                    yield return null;
                }
                SetAlphaAll(0f);

                yield return new WaitForSeconds(UnityEngine.Random.Range(_tvStaticIntervalRange.x, _tvStaticIntervalRange.y));
            }
        }

        private void SetAlphaAll(float a)
        {
            a = Mathf.Clamp01(a);
            foreach (var inst in _tvStaticInstances)
            {
                if (inst == null) continue;
                var cg = inst.GetComponent<CanvasGroup>();
                if (cg != null) { cg.alpha = a; continue; }
                var img = inst.GetComponent<Image>();
                if (img != null) { var c = img.color; c.a = a; img.color = c; }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ClearBrokens();
            StopTvStatic();
        }
    }
}
