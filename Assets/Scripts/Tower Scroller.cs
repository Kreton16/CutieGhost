using System.Collections;
using UnityEngine;

public class TowerScroller : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Sprite[] towers;
    [SerializeField] private float towerScrollSpeed = 1.0f;
    [SerializeField] private float accelerationRate = 0.1f;
    [SerializeField] private float maximalSpeed = 4.04f;
    private float updateInterval = 0.02f; // Оновлення кожні 20 мс (~50 разів на секунду)
    private float nextUpdateTime = 0f;
    private float previousSpeed;
    [SerializeField] private float speedMultiplier = 5.0f;

    public float TowerScrollSpeed => towerScrollSpeed;
    public float SpeedMultiplier => speedMultiplier;

    private PlayerUltimate playerUltimate;
    private GameObject currentZone;
    private GameObject nextZone;
    private Sprite lastUsedSprite;

    void Start()
    {
        playerUltimate = GameObject.FindWithTag("Player").GetComponent<PlayerUltimate>();
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
        previousSpeed = towerScrollSpeed;
        towerScrollSpeed *= speedMultiplier;
        Invoke(nameof(TurnUltimateOff), playerUltimate.UltimateDuration);
    }

    void TurnUltimateOff()
    {
        towerScrollSpeed = previousSpeed;
    }
}
