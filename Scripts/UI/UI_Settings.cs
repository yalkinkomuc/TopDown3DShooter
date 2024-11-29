using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float sliderMultiplier = 25f;

    [Header("SFX Settings")]

    [SerializeField] private TextMeshProUGUI sfxSliderText;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private string sfxParameter;

    [Header("BGM Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmSliderText;

    [SerializeField] private string bgmParameter;



    public void SFXSliderValue(float value)
    {
        sfxSliderText.text = Mathf.RoundToInt(value * 100) + "%";
        float newValue = Mathf.Log10(value) * sliderMultiplier;

        audioMixer.SetFloat(sfxParameter, newValue);
    }

    public void BgmSliderValue(float value)
    {
        bgmSliderText.text = Mathf.RoundToInt(value * 100) + "%";
        float newValue = Mathf.Log10(value)*sliderMultiplier;
        audioMixer.SetFloat(bgmParameter, newValue);
    }

    public void LoadSettings()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter,.7f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, .7f);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(sfxParameter, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParameter, bgmSlider.value);

    }
}
