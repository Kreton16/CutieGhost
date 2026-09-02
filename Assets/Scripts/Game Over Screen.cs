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
    [SerializeField] GameObject sessionStatsBox;
    public static GameOverScreen Instance;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI bestScore;
    [SerializeField] private TextMeshProUGUI currencySouls;
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

    public void Setup(float distance, float record, float currency)
    {
        score.text = "Score: " + distance.ToString() + " m";
        bestScore.text = "Best Score: " + record.ToString() + " m";
        currencySouls.text = "Currency: " + currency.ToString();
    }

    public void Pause()
    {
        deathMenu.SetActive(true);
        pauseButton.SetActive(false);
        sessionStatsBox.SetActive(false);


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
