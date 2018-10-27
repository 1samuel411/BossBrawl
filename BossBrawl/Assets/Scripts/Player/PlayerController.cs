using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{


    public float moveSpeed;

    public float rollDistance;
    private Rigidbody rb;
    public float rollCD;
    private float coolDown;

    public Health health;

    private bool isGrounded = true;
    public float jumpHeight;





    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        health.onDamage += OnDamaged;

    }

    void OnDamaged(int healthLost)
    {
        Debug.Log(health.currentHealth);
    }

    // Update is called once per frame
    void Update()
    {

        var x = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        var y = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;

        transform.Translate(0, 0, x);
        transform.Translate(y, 0, 0);

        // coolDown = Time.time + rollCD;




        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (coolDown <= Time.time)
            {
                if (isGrounded == true)
                {
                    rb.velocity = transform.forward * rollDistance;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            health.TakeDamage(5);
        }

    }

    private void jump()
    {
        if (isGrounded == true)
        {
            rb.AddForce(Vector3.up * jumpHeight);
        }
    }


    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }

    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }

        if (other.gameObject.tag == "Lava")
        {
            jump();
            isGrounded = true;
            health.TakeDamage(5);
            Debug.Log("You are in Lava");
        }
    }


}







