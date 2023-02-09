using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public AudioClip[] audioClips;
    public AudioSource audioSource;
    public AudioSource pitchableAudioSource;

    [ContextMenu("Test Play Sound")]
    void TestPlaySound() => PlaySound(audioClips[0].name);

    public void PlaySound(SoundInfo sound) {
        var source = sound.pitch != 1 ? pitchableAudioSource : audioSource; 
        source.pitch = sound.pitch;
        var audioClip = Array.Find(audioClips, a => {
            return a && a.name.Equals(sound.name, StringComparison.OrdinalIgnoreCase);
        });
        source.PlayOneShot(audioClip, sound.volume);
    }
}
