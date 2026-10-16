using UnityEngine;

public abstract class BaseSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected float minSpawnInterval = 1.0f;
    [SerializeField] protected float maxSpawnInterval = 3.0f;
    [SerializeField] protected float zoneWidth = 6.0f;
    [SerializeField] protected float spawnOffsetY = 15.0f;
    [SerializeField] private float spawnSpeedInfluence = 0.4f;

    protected float nextSpawnTime;

    protected PlayerUltimate playerUltimate;
    protected TowerScroller towerController;

    protected virtual void Start()
    {
        playerUltimate = GameObject.FindWithTag("Player").GetComponent<PlayerUltimate>();
        towerController = GameObject.FindWithTag("Tower Scroller").GetComponent<TowerScroller>();

        SetNextSpawnTime();
    }

    protected virtual void Update()
    {
        nextSpawnTime -= Time.deltaTime;

        if (nextSpawnTime <= 0)
        {
            if (!playerUltimate.UltimateActivated)
            {
                Spawn();
            }
            SetNextSpawnTime();
        }
    }

    protected void SetNextSpawnTime()
    {
        float speedRatio = towerController.TowerScrollSpeed / 1f;

        float multiplier = 1f + (speedRatio - 1f) * spawnSpeedInfluence;

        float currentMinInterval = minSpawnInterval / multiplier;
        float currentMaxInterval = maxSpawnInterval / multiplier;
        float baseInterval = Random.Range(currentMinInterval, currentMaxInterval);
        nextSpawnTime = baseInterval;
    }

    protected abstract void Spawn();
}
