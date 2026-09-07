using System.Collections;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{

    // Variables for speed system
    [Header("Speed Settings")]
    [SerializeField] private InputAction MoveAction;
    [SerializeField] private Rigidbody2D rigidbody2d;
    [SerializeField] private float speed;
    private float previousSpeed;
    [SerializeField] private Vector2 move;
    Vector2 previousMoveDirection;

    [Header("Ultimate Settings")]
    [SerializeField] private PlayerUltimate playerUltimate;

    // Variables for slow down
    [SerializeField] private float timeSlowed;

    // Variables for soul system
    [Header("Collectibles Settings")]
    [SerializeField] private int maxPoints = 100;
    [SerializeField] private int currentPoints;
    public int points { get { return currentPoints; } }
    [SerializeField] private int currencySouls;
    private string currentState;
    public int MaxPoints => maxPoints;
    public int CurrencySouls => currencySouls;

    // Variables related to temporary invincibility
    [Header("Invincibility Settings")]
    [SerializeField] private float timeInvincible;
    private bool isInvincible;
    private float damageCooldown;
    public bool IsInvincible => isInvincible;

    // Variables for distance counter and display
    [Header("UI Settings")]
    [SerializeField] private float recordDistance;
    private float distance;
    TowerScroller towerController;
    [SerializeField] private GameObject towerScroller;
    [SerializeField] private GameOverScreen GameOverScreen;
    [SerializeField] private TextMeshProUGUI currentDistance;
    [SerializeField] private TextMeshProUGUI currentCurrency;
    public float RecordDistance => recordDistance;

    // Animation
    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadPlayer();
        previousSpeed = speed;
        //Debug.Log("Previous speed: " + previousSpeed);

        towerController = towerScroller.GetComponent<TowerScroller>();
        StartCoroutine(DistanceCounter());

        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        currentPoints = Mathf.RoundToInt(maxPoints / 2);
        Debug.Log("Souls: " + currentPoints + "/" + maxPoints);

        UpdateGhostState();

        DisplayCurrency(currencySouls);
    }

    // Update is called once per frame
    void Update()
    {

        move = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
            if (move.magnitude > 0.1f)
            {
                if (moveDirection != previousMoveDirection)
                {
                    animator.SetBool("IsTurning", true); // Запуск анімації повороту
                }
            }
            previousMoveDirection = moveDirection;
            animator.SetFloat("Move X", moveDirection.x);
            animator.SetFloat("Move Y", moveDirection.y);
            animator.SetFloat("Speed", move.magnitude);
        }
        else
        {
            animator.SetFloat("Speed", move.magnitude);
        }

        // Якщо анімація TurnSide закінчилась, то переходимо назад у Moving
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("TurnSide") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.SetBool("IsTurning", false);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SavePlayer();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Currency: " + currencySouls);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Record Distance: " + recordDistance);
        }

        //Restart if there're no souls
        if (currentPoints <= 0)
        {
            SavePlayer();
            MoveAction.Disable();
            animator.Play("Death");
            MusicManager.Instance.PlayMusic("GameOver");
            if (stateInfo.IsName("Death") && stateInfo.normalizedTime >= 1.0f)
            {
                playerUltimate.UltimateParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                GameOverScreen.Setup(distance, recordDistance, currencySouls);
                GameOverScreen.Instance.Pause();

                Destroy(gameObject);

            }
            
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        InvincibilityTimer();
    }

    // FixedUpdate has the same call rate as the physics system 
    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {

        PlayerData data = SaveSystem.LoadPlayer();

        currencySouls = data.currency;
        recordDistance = data.recordDistance;

    }

    void InvincibilityTimer()
    {
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }
    }

    // Increase distance counter
    IEnumerator DistanceCounter()
    {
        while (true)
        {
            distance += 10;
            recordDistance = Mathf.Max(recordDistance, distance);

            DisplayDistance(distance);

            float interval = 1 / towerController.towerScrollSpeed;
            yield return new WaitForSeconds(interval);
        }

    }

    public void DisplayDistance(float score)
    {
        currentDistance.text = "Score: " + score.ToString() + $" ({recordDistance})";
    }

    public void DisplayCurrency(float currency)
    {
        currentCurrency.text = "Currency: " + currency.ToString();
    }

    public void AddPoints(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("AddPoints отримав недійсне значення.");
            return;
        }

        currentPoints = Mathf.Clamp(currentPoints + amount, 0, maxPoints);
        UpdateGhostState();
        Debug.Log(currentPoints + "/" + maxPoints);
    }

    public void RemovePoints(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("RemovePoints отримав недійсне значення.");
            return;
        }

        if (isInvincible || playerUltimate.UltimateActivated) return;

        isInvincible = true;
        damageCooldown = timeInvincible;
        currentPoints = Mathf.Clamp(currentPoints - amount, 0, maxPoints);

        SoundManager.Instance.PlaySound3D("Damage", transform.position);
        animator.Play("Damage_" + currentState);

        UpdateGhostState();
        Debug.Log(currentPoints + "/" + maxPoints);
    }

    public void AddCurrency(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("AddCurrency отримав недійсне значення.");
            return;
        }

        currencySouls += amount;
        DisplayCurrency(currencySouls);
        Debug.Log(currencySouls);
    }

    public void UpdateGhostState()
    {
        float soulPercentage = (float)currentPoints / maxPoints * 100;
        //Debug.Log(soulPercentage);

        if (soulPercentage < 30f)
        {
            animator.SetInteger("SoulState", 0); // Extinguished
            currentState = "Low";
        }
        else if (soulPercentage <= 70f)
        {
            animator.SetInteger("SoulState", 1); // Normal
            currentState = "Mid";
        }
        else
        {
            animator.SetInteger("SoulState", 2); // Glowing
            currentState = "Glow";
        }
        //Debug.Log(animator.GetInteger("SoulState"));

    }

    public void ChangeSpeed(float multiplier)
    {

        // Damage speed decrease
        if (multiplier < 1.0f)
        {
            if (isInvincible || playerUltimate.UltimateActivated)
            {
                return;
            }
            speed *= multiplier; // Зменшуємо швидкість у n разів
            Invoke(nameof(RestoreSpeed), timeSlowed); // Викликаємо відновлення швидкості через деякий час
        }

    }

    void RestoreSpeed()
    {
        speed = previousSpeed; // Відновлення швидкості
    }

}

