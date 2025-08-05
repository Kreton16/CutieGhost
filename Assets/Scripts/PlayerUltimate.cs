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
    [SerializeField] private float ultimateDuration;
    [SerializeField] private float holdTime = 1.0f;

    private float holdTimer;
    private bool isChargingUltimate = false;
    private bool ultimateActivated = false;

    public bool UltimateActivated => ultimateActivated;
    public float UltimateDuration => ultimateDuration;

    private void Start()
    {
        ultimateAction.Enable();
        ultimateAction.started += ctx => StartUltimate();
        ultimateAction.canceled += ctx => StopUltimate();
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
        if (ultimateCost < playerController.points)
        {
            Debug.Log("Ultimate Charge Started!");
            isChargingUltimate = true;
            ultimateActivated = false;
            holdTimer = 0f;
            SoundManager.Instance.PlaySound3D("UltCharge", transform.position, true);
        }
        else
        {
            Debug.Log("Not enough souls!");
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
        yield return new WaitForSeconds(ultimateDuration);
        ultimateActivated = false;
        SoundManager.Instance.StopSound("UltActive");
    }
}