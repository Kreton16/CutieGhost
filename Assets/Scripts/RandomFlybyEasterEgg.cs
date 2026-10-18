
using System.Collections;
using UnityEngine;

public class RandomFlyby : MonoBehaviour
{
    [Header("Flying settings")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float minWaitTime = 20f; // 7 хвилин
    [SerializeField] private float maxWaitTime = 60f; // 10 хвилин

    private IEnumerator Start()
    {
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        Vector3 startPoint = new Vector3(
            -10f,
            Random.Range(-5.2f, 5.2f),
            0f
        );

        Vector3 endPoint = new Vector3(
            10f,
            Random.Range(-5.2f, 5.2f),
            0f
        );

        transform.position = startPoint;

        while (Vector3.Distance(transform.position, endPoint) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                endPoint,
                speed * Time.deltaTime
            );

            yield return null;
        }
        GameObject.Destroy(gameObject);
    }
}