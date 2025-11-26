using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    private static AudioMixerController instance;


    public AudioMixer m_AudioMixer;
    public Slider m_MusicMasterSlider;
    public Slider m_MusicBGMSlider;
    public Slider m_MusicSFXSlider;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (m_MusicMasterSlider != null)
        {
            m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        if (m_MusicBGMSlider != null)
        {
            m_MusicBGMSlider.onValueChanged.AddListener(SetBGMVolume);
        }
        if (m_MusicSFXSlider != null)
        {
            m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    public void SetMasterVolume(float volume)
    {
        m_AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }
    public void SetBGMVolume(float volume)
    {
        m_AudioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }
    public void SetSFXVolume(float volume)
    {
        m_AudioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
}