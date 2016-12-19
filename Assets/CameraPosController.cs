using UnityEngine;
using System.Collections;

public class CameraPosController : MonoBehaviour
{
    float panTiltSpeed = 149f;
    float moveSpeed = 16f;

    float yaw = 0.0f;
    float pitch = 0.0f;


    void YawPitch()
    {
        var rate = Time.deltaTime * panTiltSpeed;

        var horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontal) > 0.6)
        {
            yaw +=  rate * horizontal;
        }

        var vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(vertical) > 0.001)
        {
            pitch -= rate * vertical;
        }

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    void Move()
    {
        var moveDirection = new Vector3(Input.GetAxis("Strafe"), 0, Input.GetAxis("Forward"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection = Vector3.ProjectOnPlane (moveDirection, Vector3.up);

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

    }

	void Update()
    {
        YawPitch();
        Move();
	}
}
