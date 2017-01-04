using UnityEngine;
using System.Collections;

public class OpacityGuage : MonoBehaviour {
    public static float opacity = 1.0f;

    bool moving = false;
    public float fadeInSpeed = 0.4f;
    public float moveOpacity = 0.3f;

	void Update () {
        if (Input.anyKey)
        {
            moving = true;
            opacity = moveOpacity;
        }
        else
        {
            moving = false;
        }

        if (!moving && opacity < 1.0f)
        {
            opacity = Mathf.Min(opacity + Time.deltaTime * fadeInSpeed, 1.0f);
        }
    }
}
