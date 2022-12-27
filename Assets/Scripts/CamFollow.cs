using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    /// <summary>
    /// The main focus for the camera
    /// </summary>
    public static GameObject target;
    /// <summary>
    /// Focus when no other is present
    /// </summary>
    public static Vector3 point;
    /// <summary>
    /// The secondary focus for the camera, whereas the camera will in it's direction
    /// </summary>
    public static GameObject secondaryTarget;
    public static float camSpeed = 3f;
    private void Update()
    {
        Vector2 f = target != null ? target.transform.position : point;
        transform.position = (Vector3)Vector2.Lerp(transform.position, f, camSpeed * Time.deltaTime) + new Vector3(0, 0, -20);
    }
}
