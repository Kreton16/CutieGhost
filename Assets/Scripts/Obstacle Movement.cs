using Unity.VisualScripting;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    private TowerScroller towerController;
    public float moveSpeed; // Швидкість руху
    public float multiplier;

    void Start()
    {
        towerController = GameObject.FindWithTag("Tower Scroller").GetComponent<TowerScroller>();
    }

    void Update()
    {
        moveSpeed = towerController.TowerScrollSpeed * multiplier;
        //Debug.Log(towerController.towerScrollSpeed);
        // Рух вниз
        transform.position -= new Vector3(0, moveSpeed * Time.deltaTime, 0);

        // Знищення, якщо об'єкт виходить за межі екрану
        if (transform.position.y < -10.0f)
        {
            Destroy(gameObject);
        }
    }
}
