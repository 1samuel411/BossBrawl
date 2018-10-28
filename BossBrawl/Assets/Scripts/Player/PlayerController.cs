using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{


    public float moveSpeed;

    public float rollDistance;
    private CharacterController cc;
    private Health health;
    public float rollCD;
    private float coolDown;
    private bool jump;
    private bool rolling;
    private float rollTimer;

    public bool isGrounded = true;
    public float jumpHeight;

    public float gravity;
    private Vector3 moveDirection;

    public bool isMoving
    {
        get
        {
            return new Vector3(cc.velocity.x, 0, cc.velocity.z).magnitude > 0f;
        }
    }

    // Use this for initialization
    void Start()
    {
        cc = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        health.onDamage += OnDamaged;

    }

    void OnDamaged(int healthLost)
    {

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = cc.isGrounded;

        if (rolling)
        {
            if (Time.time >= rollTimer)
                rolling = false;
        }
        else
        {
            if (cc.isGrounded || jump)
            {
                // We are grounded, so recalculate
                // move direction directly from axes

                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection = moveDirection * moveSpeed;

                if (Input.GetButton("Jump") || jump)
                {
                    jump = false;
                    moveDirection.y = jumpHeight;
                }
            }
        }

        // Apply gravity
        if(!rolling)
            moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);

        // Move the controller
        cc.Move(moveDirection * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (coolDown <= Time.time)
            {
                rolling = true;
                moveDirection = (transform.forward * rollDistance);
                rollTimer = Time.time + 0.3f;
                coolDown = Time.time + rollCD;
            }
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            PillarManager.instance.Test();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Lava")
        {
            jump = true;
            isGrounded = true;
            health.TakeDamage(5);
        }
    }


}







