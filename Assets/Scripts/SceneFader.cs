using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    [Header("Fade Settings")]
    [SerializeField] private GameObject faderImage;
    [SerializeField] private float fadeDuration = 1f;

    private Image image;
    private bool isLoading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        image = faderImage.GetComponent<Image>();
    }

    private void Start()
    {
        StartCoroutine(FadeOut());
    }

    public void LoadScene(string sceneName)
    {
        if (isLoading)
            return;

        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        isLoading = true;

        faderImage.SetActive(true);

        float time = 0f;
        Color color = image.color;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            color.a = Mathf.Clamp01(time / fadeDuration);
            image.color = color;

            yield return null;
        }

        // Якщо ми виходимо з Pause
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        faderImage.SetActive(true);

        float time = 0f;
        Color color = image.color;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            color.a = 1f - Mathf.Clamp01(time / fadeDuration);
            image.color = color;

            yield return null;
        }

        faderImage.SetActive(false);
        isLoading = false;
    }
}