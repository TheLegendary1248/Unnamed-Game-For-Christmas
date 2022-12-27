using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public static Vector2 pos;
    public static Player single;
    public float speed;
    public float stopRate;
    public float maxSpeed;
    public void Start()
    {
        CamFollow.target = gameObject;
        single = this;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 v = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (v.sqrMagnitude != 0) rb.AddForce(v * speed);
        else 
        {
            float rate = stopRate * Time.fixedDeltaTime;
            rb.velocity -= new Vector2(rate * Mathf.Sign(rb.velocity.x), rate * Mathf.Sign(rb.velocity.y));
        }
        //velocity = velocity * ( 1 - deltaTime * drag);
        pos = transform.position;
    }
}
