using UnityEngine;

public class CrossSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;  // Префаб перешкоди
    public float minSpawnInterval = 1.0f; // Мінімальний інтервал спавну
    public float maxSpawnInterval = 3.0f; // Максимальний інтервал спавну
    public float zoneWidth = 6.4f;        // Ширина ігрової зони
    public float spawnOffsetY = 15.0f;   // Відстань спавну вище екрану

    public LayerMask collectiblesLayer; // Вказати шар хрестів
    public float checkRadius = 3.8f; // Радіус перевірки

    private bool ultimateWasActive = false;
    private float nextSpawnTime;         // Час до наступного спавну

    private PlayerController playerController;
    private TowerScroller towerController;

    void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        towerController = GameObject.FindWithTag("Tower Scroller").GetComponent<TowerScroller>();
        // Встановлюємо перший випадковий інтервал
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update()
    {
        nextSpawnTime -= Time.deltaTime;

        if (playerController.UltimateActivated != ultimateWasActive)
        {
            ultimateWasActive = playerController.UltimateActivated;
            // Оновлюємо інтервал для наступного спавну
            SetNextSpawnTime();
        }

        if (nextSpawnTime <= 0)
        {
            SpawnObstacle();
            SetNextSpawnTime();
        }
    }

    void SetNextSpawnTime()
    {
        float baseInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = ultimateWasActive ? baseInterval / towerController.speedMultiplier : baseInterval;
    }

    void SpawnObstacle()
    {
        // Вибір сторони (ліво чи право)
        bool spawnOnLeft = Random.Range(0, 2) == 0;

        // Визначаємо X-позицію для спавну
        float spawnX = spawnOnLeft ? -zoneWidth / 2 : zoneWidth / 2;

        // Позиція спавну вище екрану
        Vector3 spawnPosition = new Vector3(spawnX, transform.position.y + spawnOffsetY, 0);

        // Створюємо перешкоду
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);

        if (!spawnOnLeft)
        {
            obstacle.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        // Передаємо швидкість руху
        ObstacleMovement movement = obstacle.GetComponent<ObstacleMovement>();
        if (movement != null)
        {
            movement.multiplier = 1.0f;
        }
    }
}
