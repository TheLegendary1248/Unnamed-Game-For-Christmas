using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRenderLerp : MonoBehaviour
{
    public GameObject target;
    Vector2 lastPos;
    float lastTime;
    private void Update()
    {
        transform.position = Vector2.Lerp(lastPos, target.transform.position, (Time.time - lastTime)/Time.fixedDeltaTime);
    }
    private void FixedUpdate()
    {
        lastPos = target.transform.position;
        lastTime = Time.time;
    }
}
