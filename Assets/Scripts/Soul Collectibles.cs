using UnityEngine;

public class SoulCollectibles : MonoBehaviour
{
    private CurrencySpawner currencySpawner;

    void Start()
    {
        currencySpawner = FindAnyObjectByType<CurrencySpawner>(); // Шукаємо об'єкт із цим компонентом
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();

        if (playerController != null && playerController.points < playerController.MaxPoints && gameObject.CompareTag("Soul"))
        {
            SoundManager.Instance.PlaySound3D("Collectibles", transform.position);
            playerController.AddPoints(1);
            Destroy(gameObject);
        }
        else
            if (playerController != null && gameObject.CompareTag("Currency"))
        {
            //Debug.Log(currencySoul);
            SoundManager.Instance.PlaySound3D("Collectibles", transform.position);
            playerController.AddCurrency(1);
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {

        if (other.CompareTag("Cross"))
        {
            if (gameObject.CompareTag("Currency"))
            {
                currencySpawner.CallSpawnExternally();
            }
            //Debug.Log($"Душа {gameObject.name} знищена хрестом {other.name}");
            Destroy(gameObject);
        }
    }

}
