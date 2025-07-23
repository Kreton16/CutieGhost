using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{

    [SerializeField] GameObject deathMenu;
    [SerializeField] GameObject pauseButton;
    public static GameOverScreen Instance;
    public TextMeshProUGUI recordDistance;
    private PlayerController playerController;

    public void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>(); // «найти контролер гравц€
        //Debug.Log("PlayerController in Start: " + playerController);
    }

    private void Awake()
    {
        Instance = this;
    }

    public void Setup(float score)
    {
        recordDistance.text = score.ToString() + " meters";
    }

    public void Pause()
    {
        deathMenu.SetActive(true);
        pauseButton.SetActive(false);

        foreach (Animator animator in FindObjectsByType<Animator>(FindObjectsSortMode.None))
        {
            //Debug.Log("animation stop");
            animator.speed = 0; // ¬≥дключаЇмо ан≥матор
        }

        Time.timeScale = 0.0f;
    }

    public void Home()
    {
        playerController.SavePlayer();

        SceneManager.LoadScene("Main Menu");
    }

    public void Restart()
    {
        playerController.SavePlayer();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
    }

}
