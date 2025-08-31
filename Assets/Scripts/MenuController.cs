using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio; // Added for AudioMixer

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private AudioMixer audioMixer; // Reference to AudioMixer

    private void Start()
    {
        setSlider("MasterVolume", masterSlider);
        setSlider("MusicVolume", musicSlider);
        setSlider("SFXVolume", sfxSlider);
        setSlider("UIVolume", uiSlider);
    }

    private void setSlider(string name, Slider slider)
    {
        float currentVolume;
        audioMixer.GetFloat(name, out currentVolume);
        slider.value = Mathf.Pow(10, currentVolume / 20);
        slider.onValueChanged.AddListener(v => SetVolume(v, name));
    }

    public void OnStartClicked()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }

    public void BackToMainMenu()
    {
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void OnOptionsButtonClickedOptions()
    {
        optionsPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    private void SetVolume(float volume, string name)
    {
        float dB = volume > 0 ? 20 * Mathf.Log10(volume) : -80f;
        audioMixer.SetFloat(name, dB);
    }
}