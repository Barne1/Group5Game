using System;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        slider.value = AudioListener.volume;
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
    }
}
