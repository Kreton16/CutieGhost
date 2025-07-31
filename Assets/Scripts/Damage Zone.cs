using UnityEngine;

public class DamageZone : MonoBehaviour
{

    void OnTriggerStay2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && gameObject.CompareTag("Cross"))
        {
            //Debug.Log(controller.maxPoints);
            controller.RemovePoints(1);
            //controller.rigidbody2d.AddForce(controller.move.normalized * -300.0f);
        }
        else
            if (controller != null && gameObject.CompareTag("Brick"))
        {
            //Debug.Log(controller.maxPoints);
            controller.ChangeSpeed(0.1f);
            controller.RemovePoints(1);
            //controller.rigidbody2d.AddForce(controller.move.normalized * -100.0f);

        }
    }
}
