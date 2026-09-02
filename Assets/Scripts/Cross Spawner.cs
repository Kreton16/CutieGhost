using UnityEngine;

public class CrossSpawner : BaseSpawner
{
    protected override void Spawn()
    {
        bool left = Random.Range(0, 2) == 0;
        float spawnX = left ? -zoneWidth / 2 : zoneWidth / 2;
        Vector3 spawnPos = new Vector3(spawnX, transform.position.y + spawnOffsetY, 0);

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        if (!left)
        {
            obj.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        var move = obj.GetComponent<ObstacleMovement>();
        if (move != null)
        {
            move.multiplier = 1.0f;
        }
    }
}
