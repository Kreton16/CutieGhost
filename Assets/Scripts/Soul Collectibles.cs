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
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.points < controller.maxPoints && gameObject.CompareTag("Soul"))
        {
            SoundManager.Instance.PlaySound3D("Collectibles", transform.position);
            controller.ChangePoints(1);
            Destroy(gameObject);
        }
        else
            if (controller != null && gameObject.CompareTag("Currency"))
        {
            //Debug.Log(currencySoul);
            SoundManager.Instance.PlaySound3D("Collectibles", transform.position);
            controller.currencySouls += 1;
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {

        if (other.CompareTag("Cross"))
        {
            if (gameObject.CompareTag("Currency"))
            {
                currencySpawner.SpawnSoul();
            }
            //Debug.Log($"Душа {gameObject.name} знищена хрестом {other.name}");
            Destroy(gameObject);
        }
    }

}
