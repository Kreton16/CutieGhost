using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseButton;
    private bool isPaused = false;
    private PlayerController playerController;
    [SerializeField] private GameObject optionsMenu;
    private MainMenuManager manager;

    public void Start()
    {
        manager = FindAnyObjectByType<MainMenuManager>();

        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>(); // «найти контролер гравц€
        Debug.Log("PlayerController in Start: " + playerController);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Pause();
            FindAnyObjectByType<MusicManager>().EnableLowPass();
        }
        else
        {
            Resume();
            FindAnyObjectByType<MusicManager>().DisableLowPass();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Debug.Log(isPaused);
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);

        foreach (Animator animator in FindObjectsByType<Animator>(FindObjectsSortMode.None))
        {
            //Debug.Log("animation stop");
            animator.speed = 0; // ¬≥дключаЇмо ан≥матор
        }

        if (playerController != null)
        {
            //Debug.Log("Paehali, norm vse");
            playerController.enabled = false;
        }

        Time.timeScale = 0.0f;
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        isPaused = false;
        Debug.Log(isPaused);
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        foreach (Animator animator in FindObjectsByType<Animator>(FindObjectsSortMode.None))
        {
            //Debug.Log("Animation ne stop");
            animator.speed = 1; // ¬микаЇмо ан≥матор
        }

        if (playerController != null)
        {
            //Debug.Log("Dvigaemsia");
            playerController.enabled = true;
        }

        if (optionsMenu.activeInHierarchy)
        {
            optionsMenu.SetActive(false);
        }
    }

    public void Home()
    {
        playerController.SavePlayer();

        FindAnyObjectByType<MusicManager>().DisableLowPass();
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1.0f;
    }

    public void Restart()
    {
        playerController.SavePlayer();

        FindAnyObjectByType<MusicManager>().DisableLowPass();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        manager.PlayGame();
        Time.timeScale = 1.0f;
    }

}
