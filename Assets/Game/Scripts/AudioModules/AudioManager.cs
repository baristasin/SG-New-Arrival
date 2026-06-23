using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

[System.Serializable]
public struct AudioParameter
{
    public string Name;
    public float Value;

    public AudioParameter(string name, float value)
    {
        Name = name;
        Value = value;
    }
}

[DefaultExecutionOrder(-1000)]
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    
    private readonly Dictionary<string, EventInstance> _noOverlapInstances = new();

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<AudioManager>();

                if (_instance == null)
                {
                    GameObject go = new GameObject("[AudioManager]");
                    _instance = go.AddComponent<AudioManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// Create Instance Hereeee
    /// </summary>
    /// <param name="eventRef"></param>
    /// <returns></returns>

    public EventInstance CreateInstance(EventReference eventRef)
    {
        return RuntimeManager.CreateInstance(eventRef);
    }

    /// <summary>
    /// One-shots for 2D & 3D actions
    /// </summary>
    /// <param name="eventRef"></param>

    public void PlayOneShot(EventReference eventRef)
    {
        RuntimeManager.PlayOneShot(eventRef);
    }

    public void PlayOneShot(EventReference eventRef, Vector3 worldPosition)
    {
        RuntimeManager.PlayOneShot(eventRef, worldPosition);
    }

    public void PlayOneShotAttached(EventReference eventRef, GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("[AudioManager] PlayOneShotAttached called with null target.");
            return;
        }

        RuntimeManager.PlayOneShotAttached(eventRef, target);
    }

    /// <summary>
    /// PlayOneShot(worldPosition) with a per-call volume multiplier. Same shape as the standard
    /// 3D one-shot helper, just with setVolume bolted on. Use this when the caller already
    /// knows where the sound should originate (e.g. weapon at player position).
    /// </summary>
    public void PlayOneShotWithVolume(EventReference eventRef, Vector3 worldPosition, float volume)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventRef);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPosition));
        instance.setVolume(volume);
        instance.start();
        instance.release();
    }

    /// <summary>
    /// One-shots with parameters
    /// FMOD helper PlayOneShot cannot set parameters,
    /// so I create manually
    /// </summary>
    /// <param name="eventRef"></param>
    /// <param name="parameters"></param>

    public void PlayOneShotWithParams(EventReference eventRef, params AudioParameter[] parameters)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventRef);
        ApplyParameters(instance, parameters);
        instance.start();
        instance.release();
    }

    public void PlayOneShotWithParams(EventReference eventRef, Vector3 worldPosition, params AudioParameter[] parameters)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventRef);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPosition));
        ApplyParameters(instance, parameters);
        instance.start();
        instance.release();
    }

    public void PlayOneShotAttachedWithParams(EventReference eventRef, GameObject target, params AudioParameter[] parameters)
    {
        if (target == null)
        {
            Debug.LogWarning("[AudioManager] [PlayOneShotAttachedWithParams] null");
            return;
        }

        EventInstance instance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(instance, target, true);
        ApplyParameters(instance, parameters);
        instance.start();
        instance.release();
    }
    
    /// <summary>
    /// No overlap 3D sound
    /// </summary>
    /// <param name="key"></param>
    /// <param name="eventRef"></param>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public void PlayOneShotNoOverlap(string key, EventReference eventRef, Vector3 worldPosition)
    {
        if (!_noOverlapInstances.TryGetValue(key, out EventInstance instance) || !instance.isValid())
        {
            instance = RuntimeManager.CreateInstance(eventRef);
            _noOverlapInstances[key] = instance;
        }

        instance.getPlaybackState(out PLAYBACK_STATE state);

        if (state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING)
        {
            return;
        }

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPosition));
        instance.start();
    }
    
    /// <summary>
    /// 2D Overlap sound
    /// </summary>
    /// <param name="key"></param>
    /// <param name="eventRef"></param>
    /// <returns></returns>
    public void PlayOneShotNoOverlap(string key, EventReference eventRef)
    {
        if (!_noOverlapInstances.TryGetValue(key, out EventInstance instance) || !instance.isValid())
        {
            instance = RuntimeManager.CreateInstance(eventRef);
            _noOverlapInstances[key] = instance;
        }

        instance.getPlaybackState(out PLAYBACK_STATE state);

        if (state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING)
        {
            return ;
        }

        instance.start();
    }
    
    /// <summary>
    /// For moving objects
    /// </summary>
    /// <param name="key"></param>
    /// <param name="eventRef"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public void PlayOneShotNoOverlapAttached(string key, EventReference eventRef, GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("[AudioManager] Target is null.");
            return;
        }

        if (!_noOverlapInstances.TryGetValue(key, out EventInstance instance) || !instance.isValid())
        {
            instance = RuntimeManager.CreateInstance(eventRef);
            _noOverlapInstances[key] = instance;
        }

        instance.getPlaybackState(out PLAYBACK_STATE state);

        if (state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING)
        {
            return;
        }

        // Attach every time before starting.
        RuntimeManager.AttachInstanceToGameObject(instance, target.transform, true);

        instance.start();

    }
    
    // ---------------------------
    // Loops / controllable sounds
    // Caller stores the returned EventInstance.
    // ---------------------------

    public EventInstance PlayLoop(EventReference eventRef, params AudioParameter[] parameters)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventRef);
        ApplyParameters(instance, parameters);
        instance.start();
        return instance;
    }

    public EventInstance PlayLoopAtPosition(EventReference eventRef, Vector3 worldPosition, params AudioParameter[] parameters)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventRef);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPosition));
        ApplyParameters(instance, parameters);
        instance.start();
        return instance;
    }

    public EventInstance PlayLoopAttached(EventReference eventRef, GameObject target, params AudioParameter[] parameters)
    {
        if (target == null)
        {
            Debug.LogWarning("[AudioManager] PlayLoopAttached called with null target.");
            return default;
        }

        EventInstance instance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(instance, target, true);
        ApplyParameters(instance, parameters);
        instance.start();
        return instance;
    }

    public void EventStart(EventInstance instance)
    {
        if (!instance.isValid()) return;
        instance.start();
    }
    
    public bool IsPlaying(EventInstance instance)
    {
        if (!instance.isValid()) return false;

        instance.getPlaybackState(out PLAYBACK_STATE state);
        return state != PLAYBACK_STATE.STOPPED;
    }

    public void SetParameter(EventInstance instance, string parameterName, float value)
    {
        if (!instance.isValid()) return;
        instance.setParameterByName(parameterName, value);
    }

    public void SetVolume(EventInstance instance, float volume)
    {
        if (!instance.isValid()) return;
        instance.setVolume(volume);
    }

    public void Pause(EventInstance instance, bool paused)
    {
        if (!instance.isValid()) return;
        instance.setPaused(paused);
    }
    
    public void StopNoOverlap(string key, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
    {
        if (!_noOverlapInstances.TryGetValue(key, out EventInstance instance) || !instance.isValid())
        {
            return;
        }

        instance.stop(stopMode);
    }

    public void ReleaseNoOverlap(string key)
    {
        if (!_noOverlapInstances.TryGetValue(key, out EventInstance instance))
        {
            return;
        }

        if (instance.isValid())
        {
            instance.stop(STOP_MODE.IMMEDIATE);
            instance.release();
        }

        _noOverlapInstances.Remove(key);
    }


    public void Stop(ref EventInstance instance, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
    {
        if (!instance.isValid()) return;

        instance.stop(stopMode);
        instance.release();
        instance = default;
    }

    public void ReleaseUnstarted(ref EventInstance instance)
    {
        if (!instance.isValid()) return;

        instance.release();
        instance = default;
    }
    
    public void PauseAll(bool paused)
    {
        RuntimeManager.PauseAllEvents(paused);
    }
    
    /// <summary>
    /// In case, I might need to use buses
    /// </summary>
    /// <param name="busPath"></param>
    /// <param name="volume"></param>

    public void SetBusVolume(string busPath, float volume)
    {
        Bus bus = RuntimeManager.GetBus(busPath);
        bus.setVolume(volume);
    }

    public void SetBusMute(string busPath, bool muted)
    {
        Bus bus = RuntimeManager.GetBus(busPath);
        bus.setMute(muted);
    }

    private static void ApplyParameters(EventInstance instance, AudioParameter[] parameters)
    {
        if (parameters == null) return;

        for (int i = 0; i < parameters.Length; i++)
        {
            instance.setParameterByName(parameters[i].Name, parameters[i].Value);
        }
    }
}