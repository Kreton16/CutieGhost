using UnityEngine;

public class SoulSpawner : BaseSpawner
{
    protected override void Spawn()
    {
        float spawnX = Random.Range(-zoneWidth / 2, zoneWidth / 2);
        Vector3 spawnPos = new Vector3(spawnX, transform.position.y + spawnOffsetY, 0);

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        var move = obj.GetComponent<ObstacleMovement>();
        if (move != null)
        {
            move.multiplier = 1.0f;
        }
    }
}
