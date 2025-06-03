using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AutoButtonSFX : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioMixerGroup sfxMixerGroup;

    private AudioSource sharedSource;

    void Start()
    {
        sharedSource = gameObject.AddComponent<AudioSource>();
        sharedSource.clip = clickSound;
        sharedSource.outputAudioMixerGroup = sfxMixerGroup;
        sharedSource.playOnAwake = false;

        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() =>
            {
                if (sharedSource != null && sharedSource.enabled)
                    sharedSource.PlayOneShot(clickSound);
            });
        }
    }
}
