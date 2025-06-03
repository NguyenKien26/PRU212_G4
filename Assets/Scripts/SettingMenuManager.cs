using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
public class SettingMenuManager : MonoBehaviour
{
    public Slider masterVol, musicVol, sfxVol;
    public AudioMixer mainAudiomixer;
    public GameObject settingPopup;

    public void ChangeMasterVolume()
    {
        mainAudiomixer.SetFloat("MasterVolume",masterVol.value); 
    }
    public void ChangeMusicVolume()
    {
        mainAudiomixer.SetFloat("MusicVolume", musicVol.value);
    }
    public void ChangeSFXVolume()
    {
        mainAudiomixer.SetFloat("SFXVolume", sfxVol.value);
    }
    public void OnBackButtonPressed()
    {
        settingPopup.SetActive(false);
    }
}
