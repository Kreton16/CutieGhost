using UnityEngine;

public class FlagsSpawner : BaseSpawner
{
    public Sprite[] flags;
    private Sprite lastUsedSprite;

    protected override void Update()
    {

        nextSpawnTime -= Time.deltaTime;

        if (nextSpawnTime <= 0)
        {
            Spawn();
            SetNextSpawnTime();
        }
    }

    protected override void Spawn()
    {
        bool left = Random.Range(0, 2) == 0;
        float spawnX = left ? -zoneWidth / 2 : zoneWidth / 2;
        Vector3 spawnPos = new Vector3(spawnX, transform.position.y + spawnOffsetY, 0);

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        var move = obj.GetComponent<ObstacleMovement>();
        if (move != null)
        {
            move.multiplier = 1.0f;
        }
        AssignRandomSprite(prefab);
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
            int randomIndex = Random.Range(0, flags.Length);
            newSprite = flags[randomIndex];
        } while (newSprite == lastUsedSprite);

        renderer.sprite = newSprite;
        lastUsedSprite = newSprite;
    }

}
