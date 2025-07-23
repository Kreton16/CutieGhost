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
    public InputAction MoveAction;
    public Rigidbody2D rigidbody2d;
    public float speed;
    private float previousSpeed;
    public Vector2 move;
    Vector2 previousMoveDirection;

    // Variables for ultimate skill
    public InputAction Ultimate;
    private float holdTime = 1.0f; // Час утримання для активації Ultimate
    private float holdTimer;
    private bool isChargingUltimate = false;
    public bool ultimateActivated;
    public int ultimateCost;
    public float ultimateDuration;

    // Variables for slow down
    public float timeSlowed;

    // Variables for soul system
    public int maxPoints = 100;
    int currentPoints;
    public int points { get { return currentPoints; } }
    public int currencySouls;
    private string currentState;

    // Variables related to temporary invincibility
    public float timeInvincible;
    bool isInvincible;
    float damageCooldown;

    // Variables for distance counter and display
    private float distance;
    public float recordDistance;
    TowerScroller towerController;
    public GameObject otherObject;
    public GameOverScreen GameOverScreen;
    public TextMeshProUGUI currentDistance;

    // Animation
    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadPlayer();
        previousSpeed = speed;
        //Debug.Log("Previous speed: " + previousSpeed);

        towerController = otherObject.GetComponent<TowerScroller>();
        StartCoroutine(DistanceCounter());

        Ultimate.Enable();
        Ultimate.started += ctx => StartUltimate(); // Початок утримання
        Ultimate.canceled += ctx => StopUltimate(); // Відпускання кнопки

        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        currentPoints = Mathf.RoundToInt(maxPoints / 2);
        Debug.Log("Souls: " + currentPoints + "/" + maxPoints);

        UpdateGhostState();
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

        if (isChargingUltimate)
        {
            ChargeUltimate();
        }

        //Restart if there're no souls
        if (currentPoints <= 0)
        {
            SavePlayer();
            MoveAction.Disable();
            animator.Play("Death");
            if (stateInfo.IsName("Death") && stateInfo.normalizedTime >= 1.0f)
            { 

                GameOverScreen.Setup(recordDistance);
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
        currentDistance.text = "Distance: " + score.ToString() + $" ({recordDistance})";
    }

    public void TakingDamage(int damage)
    {
        if (isInvincible || ultimateActivated)
        {
            return;
        }
        isInvincible = true;
        damageCooldown = timeInvincible;
        ChangePoints(-damage);
        SoundManager.Instance.PlaySound3D("Damage", transform.position);

        string damageAnim = "Damage_" + currentState;
        animator.Play(damageAnim);

    }

    public void ChangePoints(int amount)
    {
        currentPoints = Mathf.Clamp(currentPoints + amount, 0, maxPoints);
        UpdateGhostState();
        Debug.Log(currentPoints + "/" + maxPoints);
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

    void StartUltimate()
    {
        if (ultimateCost < points)
        {
            Debug.Log("Ultimate Charge Started!");
            isChargingUltimate = true;
            ultimateActivated = false;
            holdTimer = 0f;
            SoundManager.Instance.PlaySound3D("UltCharge", transform.position);
        }
        else
        {
            Debug.Log("Not enough souls!");
        }
    }

    void ChargeUltimate()
    {
        if (isChargingUltimate && !ultimateActivated && ultimateCost < points)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdTime)
            {
                ChangePoints(-ultimateCost);
                ActivateUltimate();
                isChargingUltimate = false;
                return;
            }
        }
        else 
        {
            StopUltimate();
        }
    }

    void ActivateUltimate()
    {
        if (isChargingUltimate && holdTimer >= holdTime)
        {
            Debug.Log("Ultimate Activated!");
            SoundManager.Instance.StopSound("UltCharge");
            SoundManager.Instance.PlaySound3D("UltActive", transform.position, true);
            Debug.Log("Ultimate Activated!");
            ultimateActivated = true;
            CostChange();
            towerController.ChangeSpeed();
            StartCoroutine("TurnUltimateOff");
        }
    }

    void StopUltimate()
    {
        if(!ultimateActivated && isChargingUltimate)
        {
            Debug.Log("Ultimate canceled - hold longer!");
            SoundManager.Instance.StopSound("UltCharge");
        }

        isChargingUltimate = false;
    }

    void CostChange()
    {
        ultimateCost = Mathf.Clamp(ultimateCost + 20, 0, maxPoints);
    }

    public void ChangeSpeed(float multiplier)
    {
        // Ultimate speed boost
        if (multiplier > 1.0f && ultimateActivated)
        {
            speed *= multiplier; // Збільшуємо швидкість у n разів
            Invoke(nameof(RestoreSpeed), ultimateDuration); // Викликаємо відновлення швидкості через деякий час
            //StartCoroutine("TurnUltimateOff");
        }

        // Damage speed decrease
        if (multiplier < 1.0f)
        {
            if (isInvincible)
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

    IEnumerator TurnUltimateOff()
    {
        yield return new WaitForSeconds(ultimateDuration);
        ultimateActivated = false;
        SoundManager.Instance.StopSound("UltActive");
    }

}

