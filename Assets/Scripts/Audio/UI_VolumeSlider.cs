using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_VolumeSlider : MonoBehaviour
{
    public Slider slider;
    public string volumeType;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private int multiplier;

    public void SliderValueChange(float _value)
    {
        audioMixer.SetFloat(volumeType, Mathf.Log10(_value) * multiplier);
    }


    public void LoadVolumeValue(float _value)
    {
        if (_value >= 0.001f)
        {
            slider.value = _value;
            // 在load的时候也调用一次，确保audioMixer音量生效
            SliderValueChange(_value);
        }
    }
}