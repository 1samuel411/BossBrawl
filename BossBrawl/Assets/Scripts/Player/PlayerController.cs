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
    public bool jump;
    private bool rolling;
    private float rollTimer;

    public bool isGrounded = true;
    public float jumpHeight;

    public float gravity;
    private Vector3 moveDirection;

    public Animator animator;
    public Transform sphereCast;
    public float radius;
    public float amount;
    public LayerMask layerMask;

    private float attackTimer;
    public bool attacking;
    private float damageTimer;

    public OnTriggerForward triggerForward;
    public float damage = 8f;

    private bool dead;

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
        health.onDeath += OnDead;
        triggerForward.eventForwardDelegate += OnEnterEnemy;
        triggerForward.eventExitForwardDelegate += OnExitEnemy;
    }

    void OnDamaged(int healthLost)
    {

    }

    void OnDead()
    {
        animator.SetBool("Dead", true);
        //GameOver.instance.Death();
        dead = true;
        moveDirection = Vector3.zero;
    }

    private bool lastGrounded;
    private float timeSinceLastGrounded;
    // Update is called once per frame
    void Update()
    {
        if (dead)
            return;


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

                moveDirection = new Vector3(InputManager.instance.player2.GetAxis("XMovement"), 0.0f, InputManager.instance.player2.GetAxis("YMovement"));
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection = moveDirection * moveSpeed;

                if (InputManager.instance.player2.GetButton("Jump") || jump)
                {
                    animator.SetTrigger("Jump");
                    jump = false;
                    moveDirection.y = jumpHeight;
                }
            }
        }

        if(InputManager.instance.player2.GetButtonDown("Attack1") && Time.time >= attackTimer)
        {
            attackTimer = Time.time + 0.55f;
            animator.SetTrigger("Attack");
            attacking = true;
            damageTimer = Time.time + 0.15f;
        }

        if(attacking && Time.time >= damageTimer)
        {
            attacking = false;
            Damage();
        }

        Vector3 rawInput = transform.InverseTransformDirection(moveDirection);
        animator.SetFloat("XInput", rawInput.x);
        animator.SetFloat("ZInput", rawInput.z);
        

        RaycastHit hit;
        bool grounded = (Physics.SphereCast(sphereCast.position, radius, Vector3.down, out hit, amount, layerMask));
        if (grounded != lastGrounded)
        {
            lastGrounded = isGrounded;
            animator.SetTrigger("GroundedChange");
            timeSinceLastGrounded = Time.time + 0.5f;
        }
        animator.SetBool("Grounded", grounded);

        // Apply gravity
        if (!rolling)
            moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);

        // Move the controller
        cc.Move(moveDirection * Time.deltaTime);
        /*
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (coolDown <= Time.time)
            {
                rolling = true;
                moveDirection = (transform.forward * rollDistance);
                rollTimer = Time.time + 0.3f;
                coolDown = Time.time + rollCD;
            }
        }*/
    }

    public GameObject enemy;
    void OnEnterEnemy(Collider col)
    {
        enemy = col.gameObject;
    }

    void OnExitEnemy(Collider col)
    {
        enemy = null;
    }

    void Damage()
    {
        if (enemy == null)
            return;

        Health enemyHealth = enemy.GetComponentInParent<Health>();
        int dmg = (int)(damage * Random.Range(1.0f, 2.0f));
        enemyHealth.TakeDamage(dmg);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Lava")
        {
            jump = true;
            isGrounded = true;
            health.TakeDamage(10);
        }
    }
}