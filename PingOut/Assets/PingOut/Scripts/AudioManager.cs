using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private const string MASTER_VOLUME = "MasterVolume";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SFX_VOLUME = "SFXVolume";

    void Start()
    {
        // Initialiser les valeurs des sliders avec les valeurs sauvegardées (si nécessaire)
        if (PlayerPrefs.HasKey(MUSIC_VOLUME))
        {
            musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME);
        }
        if (PlayerPrefs.HasKey(SFX_VOLUME))
        {
            sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME);
        }
        if (PlayerPrefs.HasKey(MASTER_VOLUME))
        {
            masterSlider.value = PlayerPrefs.GetFloat(MASTER_VOLUME);
        }

        // Appeler les méthodes pour appliquer les volumes initiaux
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
        SetMasterVolume(masterSlider.value);
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        float db = Mathf.Log10(volume) * 20;

        if (volume == 0)
        {
            db = -80f;
        }

        audioMixer.SetFloat(MUSIC_VOLUME, db);
        PlayerPrefs.SetFloat(MUSIC_VOLUME, volume);
    }

    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        float db = Mathf.Log10(volume) * 20;

        if (volume == 0)
        {
            db = -80f;
        }

        audioMixer.SetFloat(SFX_VOLUME, db);
        PlayerPrefs.SetFloat(SFX_VOLUME, volume);
    }

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        float db = Mathf.Log10(volume) * 20;

        if (volume == 0)
        {
            db = -80f;
        }

        audioMixer.SetFloat(MASTER_VOLUME, db);
        PlayerPrefs.SetFloat(MASTER_VOLUME, volume);
    }
}