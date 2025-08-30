using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio; // Added for AudioMixer

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private AudioMixer audioMixer; // Reference to AudioMixer

    private void Start()
    {
        if (audioMixer != null && masterSlider != null)
        {
            float currentVolume;
            audioMixer.GetFloat("MasterVolume", out currentVolume);
            masterSlider.value = Mathf.Pow(10, currentVolume / 20);
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }
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

    private void SetMasterVolume(float volume)
    {
        float dB = volume > 0 ? 20 * Mathf.Log10(volume) : -80f;
        audioMixer.SetFloat("MasterVolume", dB);
    }
}