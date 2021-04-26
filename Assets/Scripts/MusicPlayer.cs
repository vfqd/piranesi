using System;
using SoundManager;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public EffectSoundBank music;

    private void Start()
    {
        var si = music.Play();
        si.SetLooping(true);
    }
}