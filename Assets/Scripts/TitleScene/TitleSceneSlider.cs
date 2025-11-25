using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneSlider : MonoBehaviour
{
    public AudioSource BGM;

    public void SetMusicVolume(float volume)
    {
        BGM.volume = volume;
    }
}
