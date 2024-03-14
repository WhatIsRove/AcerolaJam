using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

    int swordHit;
    int swordWoosh;
    int punch;
    int splatter;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }


        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixer;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) Play("MenuMusic");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s.name == "SwordHit" + swordHit && s.source.isPlaying)
        {
            if (swordHit == 5) swordHit = 0;
            else swordHit++;
            Play("SwordHit" + swordHit);
            return;
        }
        else if (s.name == "SwordWoosh" + swordWoosh && s.source.isPlaying)
        {
            if (swordWoosh == 4) swordWoosh = 0;
            else swordWoosh++;
            Play("SwordWoosh" + swordWoosh);
            return;
        }
        else if (s.name == "Punch" + punch && s.source.isPlaying)
        {
            if (punch == 1) punch = 0;
            else punch++;
            Play("Punch" + punch);
            return;
        }
        else if (s.name == "Splatter" + splatter && s.source.isPlaying)
        {
            if (splatter == 1) splatter = 0;
            else splatter++;
            Play("Splatter" + splatter);
            return;
        }
        else if (s.name == "Music" && s.source.isPlaying) return;
        if (s == null) return;


        if (s.rangePitch)
        {
            s.source.pitch = UnityEngine.Random.Range(s.minPitch, s.maxPitch);
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Stop();
    }
}
