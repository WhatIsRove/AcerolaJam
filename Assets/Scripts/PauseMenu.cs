using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider soundSlider;
    public Slider musicSlider;

    void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume") && PlayerPrefs.HasKey("soundVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetSoundVolume();
        }
    }

    public void SetSoundVolume()
    {
        float volume = soundSlider.value;
        mixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("soundVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        mixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void LoadVolume()
    {
        soundSlider.value = PlayerPrefs.GetFloat("soundVolume");
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetSoundVolume();
        SetMusicVolume();
    }
}
