using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private Slider bgVolumeSlider;    // Slider for BG Music
    [SerializeField] private Slider sfxVolumeSlider;   // Slider for SFX
    [SerializeField] private AudioMixer audioMixer;    // Reference to your Audio Mixer

    private void Start()
    {
        if (!PlayerPrefs.HasKey("bgVolume"))
        {
            PlayerPrefs.SetFloat("bgVolume", 1);
            Load("bgVolume", bgVolumeSlider);
        }
        else
        {
            Load("bgVolume", bgVolumeSlider);
        }

        if (!PlayerPrefs.HasKey("sfxVolume"))
        {
            PlayerPrefs.SetFloat("sfxVolume", 1);
            Load("sfxVolume", sfxVolumeSlider);
        }
        else
        {
            Load("sfxVolume", sfxVolumeSlider);
        }
    }

    public void ChangeBGVolume()
    {
        float volume = Mathf.Log10(bgVolumeSlider.value) * 20; // Convert slider value to decibels
        audioMixer.SetFloat("MusicVolume", volume);                 // Set AudioMixer group "Music"
        Save("bgVolume", bgVolumeSlider.value);
    }

    public void ChangeSFXVolume()
    {
        float volume = Mathf.Log10(sfxVolumeSlider.value) * 20; // Convert slider value to decibels
        audioMixer.SetFloat("SFXVolume", volume);                   // Set AudioMixer group "SFX"
        Save("sfxVolume", sfxVolumeSlider.value);
    }

    public void Save(string volume, float value)
    {
        PlayerPrefs.SetFloat(volume, value);
        PlayerPrefs.Save();
    }

    public void Load(string volume, Slider slider)
    {
        slider.value = PlayerPrefs.GetFloat(volume);
    }
}