using System;
using System.Collections;
using Game.Scripts.Utilities;
using UnityEngine.SceneManagement;

namespace Game.Scripts
{
    // Persistent wrapper around SceneManager.LoadSceneAsync. Exposes a progress event the
    // LoadingScreenUI listens to. Single active scene model — every load fully replaces the
    // previous scene. Persistent singletons survive via DontDestroyOnLoad on their root.
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        public const string BootstrapScene = "BootstrapScene";
        public const string DayCityScene   = "DayCity";
        public const string NightCityScene = "NightCity";

        public event Action<float> OnProgress;     // 0..1 across the async load
        public event Action OnLoadStarted;
        public event Action OnLoadFinished;

        public bool IsLoading { get; private set; }

        public IEnumerator LoadAsync(string sceneName)
        {
            if (IsLoading) yield break;
            IsLoading = true;
            OnLoadStarted?.Invoke();

            var op = SceneManager.LoadSceneAsync(sceneName);
            while (op != null && !op.isDone)
            {
                OnProgress?.Invoke(op.progress);
                yield return null;
            }

            OnProgress?.Invoke(1f);
            OnLoadFinished?.Invoke();
            IsLoading = false;
        }
    }
}
