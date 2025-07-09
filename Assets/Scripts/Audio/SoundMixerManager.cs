using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundFXVolumeSlider;

    private void Start()
    {
        float masterVolume = PlayerPrefs.GetFloat("masterVolume", 0.5f);
        masterVolumeSlider.value = masterVolume;

        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        musicVolumeSlider.value = musicVolume;

        float soundFXVolume = PlayerPrefs.GetFloat("soundFXVolume", 0.5f);
        soundFXVolumeSlider.value = soundFXVolume;
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("masterVolume", masterVolumeSlider.value);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("soundFXVolume", soundFXVolumeSlider.value);
    }
}
