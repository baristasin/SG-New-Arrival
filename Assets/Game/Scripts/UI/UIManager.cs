using Game.Scripts.SanityModules;
using Game.Scripts.Utilities;
using UnityEngine;

namespace Game.Scripts.UI
{
    // Persistent, global UI hub. Its Canvas survives scene loads and hosts shared UI — the sanity
    // bar now, settings and other panels later. Reachable from anywhere via UIManager.Instance.
    public class UIManager : PersistentSingleton<UIManager>
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private SanityBar _sanityBar;

        public Canvas Canvas => _canvas;
        public SanityBar SanityBar => _sanityBar;
    }
}
