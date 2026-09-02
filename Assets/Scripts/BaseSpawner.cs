using UnityEngine;

public abstract class BaseSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected float minSpawnInterval = 1.0f;
    [SerializeField] protected float maxSpawnInterval = 3.0f;
    [SerializeField] protected float zoneWidth = 6.0f;
    [SerializeField] protected float spawnOffsetY = 15.0f;

    protected bool ultimateWasActive = false;
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

        if (playerUltimate.UltimateActivated != ultimateWasActive)
        {
            ultimateWasActive = playerUltimate.UltimateActivated;
            SetNextSpawnTime();
        }

        if (nextSpawnTime <= 0)
        {
            Spawn();
            SetNextSpawnTime();
        }
    }

    protected void SetNextSpawnTime()
    {
        float baseInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = ultimateWasActive ? baseInterval / towerController.speedMultiplier : baseInterval;
    }

    protected abstract void Spawn();
}
