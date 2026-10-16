using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public Transform[] backgrounds;  // Масив фонів (3 штуки)
    public float backScrollSpeed = 2.0f; // Швидкість опускання
    public float decelerationRate = 0.1f; // Наскільки швидко зменшується швидкість
    public float minimalSpeed = 0.05f; // Мінімальна швидкість
    private bool backStopScrolling = false;
    private float updateInterval = 0.02f; // Оновлювати рух раз на 20 мс (~50 разів на секунду)
    private float nextUpdateTime = 0f;

    void Update()
    {
        if (backStopScrolling) return;

        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;

            // Поступово зменшуємо швидкість падіння
            if(backScrollSpeed > minimalSpeed)
            {
                Deceleration();
            }

            // Рухаємо всі фони вниз
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].position -= new Vector3(0, backScrollSpeed * updateInterval, 0);
            }
        }

        // Перевірка чи останній фон досяг кінця
        if (backgrounds[backgrounds.Length - 1].position.y <= -21.6)
        {
            backStopScrolling = true;
            return;
        }
    }

    void Deceleration() {

        backScrollSpeed -= decelerationRate * updateInterval;
        //Debug.Log(scrollSpeed);
        //scrollSpeed = Mathf.Max(scrollSpeed, minimalSpeed); // Мінімальна швидкість
    }

}
