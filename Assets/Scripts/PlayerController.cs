using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float maxSpeed;
    public float turnSpeed;

    public float decelRate;

    Rigidbody rb;
    Vector3 moveDir;

    public Animator anim;

    public LayerMask groundLayer;
    public Transform groundPoint;
    RaycastHit screenHit;
    bool rayHit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();


        
    }

    private void Update()
    {
        
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

        float dotForward = 0;

        if (rb.velocity.magnitude > 0.5)
        {
            dotForward = Vector3.Dot(rb.velocity.normalized, transform.forward.normalized);
        }
        
        anim.SetFloat("zVel", dotForward);
    }

    private void LateUpdate()
    {
        if (rayHit)
        {
            var dir = screenHit.point - groundPoint.position;
            transform.forward = dir;
        }
    }

    void OnMove(InputValue input)
    {
        var temp = input.Get<Vector2>();
        moveDir = new Vector3(temp.x, 0, temp.y);

        
    }

    void OnLook(InputValue input)
    {
        var temp = input.Get<Vector2>();
        var ray = Camera.main.ScreenPointToRay(new Vector3(temp.x, temp.y, 0));

        if (Physics.Raycast(ray, out screenHit, Mathf.Infinity, groundLayer))
        {
            rayHit = true;
        } else
        {
            rayHit = false;
        }
    }
}
