using UnityEngine;
using System.Collections;

public class CameraPosController : MonoBehaviour
{
    float panTiltSpeed = 64f;

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

	void Update()
    {
        YawPitch();
	}
}
