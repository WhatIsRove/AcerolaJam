using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float maxSpeed;
    public float turnSpeed;

    public float decelRate;

    Rigidbody rb;
    Vector3 moveDir;

    Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        rb.velocity += moveDir.normalized * moveSpeed;

        if (rb.velocity.magnitude > maxSpeed)
        {
            var localVelocity = rb.velocity;
            localVelocity.x = Mathf.Clamp(localVelocity.x, -maxSpeed, maxSpeed);
            localVelocity.z = Mathf.Clamp(localVelocity.z, -maxSpeed, maxSpeed);
            rb.velocity = localVelocity;
        }

        if (moveDir.x == 0)
        {
            var localVelocity = rb.velocity;
            localVelocity.x /= decelRate;
            rb.velocity = localVelocity;
        }

        if (moveDir.z == 0)
        {
            var localVelocity = rb.velocity;
            localVelocity.z /= decelRate;
            rb.velocity = localVelocity;
        }
    }

    void OnMove(InputValue input)
    {
        var temp = input.Get<Vector2>();
        moveDir = new Vector3(temp.x, 0, temp.y);

        if (moveDir.magnitude > 0)
        {
            anim.SetFloat("moveX", moveDir.x);
            anim.SetFloat("moveY", moveDir.z);
        }
        
    }
}
