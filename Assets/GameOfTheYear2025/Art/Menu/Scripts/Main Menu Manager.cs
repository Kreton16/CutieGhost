using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        LoadVolume();
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            MusicManager.Instance.PlayMusic("MainMenu");
        }
    }

    public void PlayGame()
    {
        // Замініть "GameScene" на назву твоєї ігрової сцени!
        SceneFader.Instance.LoadScene("MainScene");
        MusicManager.Instance.PlayMusic("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void UpdateSoundVolume(float volume)
    {
        SoundManager.Instance.SetSFXVolume(volume);
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save();
    }

    public void LoadVolume()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0f);

        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        audioMixer.SetFloat("MusicVolume", musicVol);
        SoundManager.Instance.SetSFXVolume(sfxVol);
    }
}
