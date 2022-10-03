using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : Singleton<AudioHandler>
{
    [SerializeField] private AudioSource source;
    [SerializeField] private List<AudioClip> clips;

    public void Play(AudioType type) {
        source.PlayOneShot(clips[(int)type]);
    }
}

public enum AudioType {
    HURT, PARRY, DEATH, SELECT, FIREBALL, DRONEATTACK, GUARDATTACK, ASSASSINATTACK, NOENERGY, EMP, REFILL, EVAC, TELEPORT, 
}
