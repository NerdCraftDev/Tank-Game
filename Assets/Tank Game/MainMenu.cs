using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject soundSettingsCanvas;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;

    public bool isMainMenuOpen = false;

    private void Start()
    {
        // Initialize menu states
        mainMenuCanvas.SetActive(false);
        soundSettingsCanvas.SetActive(false);
        SetMasterVolume();
    }

    private void Update()
    {
        // Toggle main menu on Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMainMenu();
        }
    }

    public void ToggleMainMenu()
    {
        isMainMenuOpen = !isMainMenuOpen;
        mainMenuCanvas.SetActive(isMainMenuOpen);
        transform.parent.GetComponent<TankController>().enabled = !isMainMenuOpen;
        transform.parent.GetComponent<TurretController>().enabled = !isMainMenuOpen;
        soundSettingsCanvas.SetActive(false);
    }

    public void ToggleSoundSettings()
    {
        soundSettingsCanvas.SetActive(!soundSettingsCanvas.activeSelf);
    }

    public void SetMasterVolume()
    {
        float volume = masterVolumeSlider.value;
        // Set master volume based on the slider value
        AudioListener.volume = volume/100;
    }

    public void SetMusicVolume(float volume)
    {
        // Set music volume based on the slider value
        // You can use your AudioManager script to handle music volume
        // musicSource.volume = volume;
    }
}
