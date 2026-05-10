using System;
using FMODUnity;
using UnityEngine;

public class OneShotAndLoopTest : MonoBehaviour
{
    [SerializeField] private EventReference zombieGrawlingEvent;

    public void PlayZombieSound()
    {
        AudioManager.Instance.PlayOneShot(zombieGrawlingEvent, transform.position);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayZombieSound();
        }
    }
}
