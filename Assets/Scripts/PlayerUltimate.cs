using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUltimate : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TowerScroller towerScroller;

    [Header("Ultimate Settings")]
    [SerializeField] private InputAction ultimateAction;
    [SerializeField] private int ultimateCost;
    [SerializeField] private float _ultimateDuration = 6f;
    [SerializeField] private float holdTime = 1.0f;
    [SerializeField] private ParticleSystem _ultimateParticles;

    private float holdTimer;
    private bool isChargingUltimate = false;
    private bool ultimateActivated = false;

    public int UltimateCost => ultimateCost;
    public bool UltimateActivated => ultimateActivated;
    public float UltimateDuration => _ultimateDuration;
    public ParticleSystem UltimateParticles => _ultimateParticles;

    private void Start()
    {
        ultimateAction.Enable();
        ultimateAction.started += ctx => StartUltimate();
        ultimateAction.canceled += ctx => StopUltimate();

        var main = _ultimateParticles.main;
        main.duration = _ultimateDuration;
    }

    private void Update()
    {
        if (isChargingUltimate)
        {
            ChargeUltimate();
        }
    }

    private void StartUltimate()
    {
        if (ultimateCost < playerController.points && !ultimateActivated)
        {
            Debug.Log("Ultimate Charge Started!");
            isChargingUltimate = true;
            holdTimer = 0f;
            SoundManager.Instance.PlaySound3D("UltCharge", transform.position, true);
        }
        else
        {
            Debug.Log("Couldn't start charging!");
        }
    }

    private void ChargeUltimate()
    {
        if (isChargingUltimate && !ultimateActivated && ultimateCost < playerController.points && !playerController.IsInvincible)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdTime)
            {
                playerController.RemovePoints(ultimateCost);
                ActivateUltimate();
                isChargingUltimate = false;
            }
        }
        else
        {
            StopUltimate();
        }
    }

    private void ActivateUltimate()
    {
        Debug.Log("Ultimate Activated!");
        SoundManager.Instance.StopSound("UltCharge");
        SoundManager.Instance.PlaySound3D("UltActive", transform.position, true);
        ultimateActivated = true;
        CostChange();
        towerScroller.ChangeSpeed();
        StartCoroutine(TurnUltimateOff());

        //Particle start
        _ultimateParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _ultimateParticles.Play();
    }

    private void StopUltimate()
    {
        if (!ultimateActivated && isChargingUltimate)
        {
            Debug.Log("Ultimate canceled - hold longer!");
            SoundManager.Instance.StopSound("UltCharge");
        }

        isChargingUltimate = false;
    }

    private void CostChange()
    {
        ultimateCost = Mathf.Clamp(ultimateCost + 20, 0, playerController.MaxPoints);
    }

    private IEnumerator TurnUltimateOff()
    {
        yield return new WaitForSeconds(_ultimateDuration);
        ultimateActivated = false;
        SoundManager.Instance.StopSound("UltActive");
    }
}