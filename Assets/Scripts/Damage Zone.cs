using UnityEngine;

public class DamageZone : MonoBehaviour
{

    void OnTriggerStay2D(Collider2D other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        PlayerUltimate playerUltimate = other.GetComponent<PlayerUltimate>();

        if (playerController != null && gameObject.CompareTag("Cross") && !playerController.IsInvincible && !playerUltimate.UltimateActivated)
        {
            //Debug.Log(controller.maxPoints);
            playerController.ChangeSpeed(0.6f);
            playerController.RemovePoints(10);
            //controller.rigidbody2d.AddForce(controller.move.normalized * -300.0f);
        }
        else
            if (playerController != null && gameObject.CompareTag("Brick") && !playerController.IsInvincible && !playerUltimate.UltimateActivated)
        {
            //Debug.Log(controller.maxPoints);
            playerController.ChangeSpeed(0.6f);
            playerController.RemovePoints(10);
            //controller.rigidbody2d.AddForce(controller.move.normalized * -100.0f);

        }
    }
}
