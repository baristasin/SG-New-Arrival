using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.AudioModules
{
    // Pushes per-bus volumes to FMOD on boot. Lives next to MusicController in the Bootstrap
    // scene. Each entry is an FMOD bus path (open the FMOD Studio Mixer to find them) plus a
    // 0–2 multiplier. The Inspector also gets a button to push the current values without a
    // restart so you can balance levels in Play mode.
    public class AudioMixSettings : MonoBehaviour
    {
        [Serializable]
        public class BusVolume
        {
            public string Path = "bus:/";
            [Range(0f, 2f)] public float Volume = 1f;
        }

        [SerializeField] private List<BusVolume> _busVolumes = new();

        private void Start()
        {
            ApplyAll();
        }

        [Button("Apply Now")]
        public void ApplyAll()
        {
            if (AudioManager.Instance == null) return;
            for (int i = 0; i < _busVolumes.Count; i++)
            {
                var b = _busVolumes[i];
                if (string.IsNullOrWhiteSpace(b.Path)) continue;
                AudioManager.Instance.SetBusVolume(b.Path, b.Volume);
            }
        }

        // Probes a handful of common bus paths and logs which ones FMOD actually finds, so you
        // can copy the working strings into _busVolumes without opening FMOD Studio. Run this
        // in Play mode (banks have to be loaded first).
        [Button("Probe Buses (logs to console)")]
        public void ProbeBuses()
        {
            string[] candidates =
            {
                "bus:/",                            // Master (always present)
                "bus:/Master",
                "bus:/Master/Music",
                "bus:/Music",
                "bus:/Master/SFX",
                "bus:/SFX",
                "bus:/Master/SFX/Guns",
                "bus:/SFX/Guns",
                "bus:/Master/Guns",
                "bus:/Guns",
                "bus:/Master/SFX/Zombies",
                "bus:/SFX/Zombies",
                "bus:/Master/Zombies",
                "bus:/Zombies",
                "bus:/Master/SFX/Minigame",
                "bus:/Minigame",
                "bus:/Master/Player",
                "bus:/Player",
                "bus:/Master/Ambience",
                "bus:/Ambience",
                "bus:/Master/Environment",
                "bus:/Environment",
            };

            int found = 0;
            for (int i = 0; i < candidates.Length; i++)
            {
                var bus = FMODUnity.RuntimeManager.GetBus(candidates[i]);
                if (bus.isValid())
                {
                    Debug.Log($"[AudioMixSettings] FOUND  {candidates[i]}");
                    found++;
                }
            }
            Debug.Log($"[AudioMixSettings] Probe done — {found} buses found. Copy any matching " +
                      $"path into _busVolumes and tune Volume.");
        }
    }
}
