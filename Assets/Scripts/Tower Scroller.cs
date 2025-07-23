using System.Collections;
using UnityEngine;

public class TowerScroller : MonoBehaviour
{
    public GameObject towerPrefab;
    public Sprite[] towers;
    public float towerScrollSpeed = 1.0f;
    public float accelerationRate = 0.1f;
    public float maximalSpeed = 4.04f;
    private float updateInterval = 0.02f; // Оновлення кожні 20 мс (~50 разів на секунду)
    private float nextUpdateTime = 0f;
    private float previousSpeed;
    public float speedMultiplier = 5.0f;

    public PlayerController playerController;
    private GameObject currentZone;
    private GameObject nextZone;
    private Sprite lastUsedSprite;

    void Start()
    {
        // Створюємо тільки дві зони
        currentZone = Instantiate(towerPrefab, Vector3.zero, Quaternion.identity);
        nextZone = Instantiate(towerPrefab, new Vector3(0, 10.8f, 0), Quaternion.identity);

        // Присвоюємо початкові спрайти
        AssignRandomSprite(currentZone);
        AssignRandomSprite(nextZone);
    }

    void Update()
    {
        // Переміщуємо вежі вниз
        currentZone.transform.position += Vector3.down * towerScrollSpeed * Time.deltaTime;
        nextZone.transform.position += Vector3.down * towerScrollSpeed * Time.deltaTime;

        // Якщо currentZone пішла за межі екрана, то вона стає новою nextZone
        if (currentZone.transform.position.y < -10.8f)
        {
            SwapZones();
        }

        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;

            // Поступове збільшення швидкості
            if (towerScrollSpeed < maximalSpeed)
            {
                Acceleration();
            }
        }
    }

    void SwapZones()
    {
        // Міняємо зони місцями
        GameObject temp = currentZone;
        currentZone = nextZone;
        nextZone = temp;

        // Переміщуємо нову nextZone наверх
        nextZone.transform.position = currentZone.transform.position + new Vector3(0, 10.8f, 0);

        // Призначаємо новий спрайт
        AssignRandomSprite(nextZone);
    }

    void AssignRandomSprite(GameObject zone)
    {
        SpriteRenderer renderer = zone.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            Debug.LogError("SpriteRenderer не знайдено на зоні!");
            return;
        }

        Sprite newSprite;
        do
        {
            int randomIndex = Random.Range(0, towers.Length);
            newSprite = towers[randomIndex];
        } while (newSprite == lastUsedSprite);

        renderer.sprite = newSprite;
        lastUsedSprite = newSprite;
    }

    void Acceleration()
    {
        towerScrollSpeed += accelerationRate * updateInterval;
    }

    public void ChangeSpeed()
    {
            Debug.Log("AHUETb");
            previousSpeed = towerScrollSpeed;
            towerScrollSpeed *= speedMultiplier;
            Invoke(nameof(TurnUltimateOff),playerController.ultimateDuration);
    }

    void TurnUltimateOff()
    {
        Debug.Log("PIZDAAA");
        towerScrollSpeed = previousSpeed;
    }
}
