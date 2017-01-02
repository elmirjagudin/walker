using UnityEngine;
using System.Collections;

public class CameraPosController : MonoBehaviour
{
    float panTiltSpeed = 149f;
    float moveSpeed = 16f;
    float levelSpeed = 8f;

    float yaw = 0.0f;
    float pitch = 0.0f;

    public GameObject collisionMarker;

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

    void Level()
    {
        var ydelta = levelSpeed * Time.deltaTime * (Input.GetAxis("Up") - Input.GetAxis("Down"));
        transform.position += new Vector3(0, ydelta, 0);

    }

	void Update()
    {
        YawPitch();
        Move();
        Level();

        UpdateCollisionMarker();
	}

    void UpdateCollisionMarker()
	{
        RaycastHit hit;
        if (!Physics.Raycast(GetComponent<Transform>().position, Vector3.down, out hit))
        {
			collisionMarker.SetActive(false);
			return;
        }

		collisionMarker.SetActive(true);
		collisionMarker.transform.position = hit.point;
	}
}
