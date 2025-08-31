using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // Added for AudioMixer
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ResourceSystem;

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
        AudioManager.Instance.click.Play();
        Dictionary<ResourceType, int> newResources = new Dictionary<ResourceType, int>
        {
            [ResourceType.None] = 0,
            [ResourceType.Supplies] = 80,
            [ResourceType.People] = 10,
            [ResourceType.Valuables] = 25,
            [ResourceType.Gear] = 50
        };

        resourceSystemReset(defaultResources);
        SceneManager.LoadSceneAsync("MainScene");
    }

    public void BackToMainMenu()
    {
        AudioManager.Instance.click.Play();
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void OnOptionsButtonClickedOptions()
    {
        AudioManager.Instance.click.Play();
        optionsPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void QuitButtonClicked()
    {
        AudioManager.Instance.click.Play();
        Application.Quit();
    }

    private void SetVolume(float volume, string name)
    {
        float dB = volume > 0 ? 20 * Mathf.Log10(volume) : -80f;
        audioMixer.SetFloat(name, dB);
    }
}