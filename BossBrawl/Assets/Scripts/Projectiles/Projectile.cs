using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public GameObject mesh;
    public GameObject[] impact;
    public new ParticleSystem particleSystem;
    public float impactKillTime;
    private new Rigidbody rigidbody;
    private new Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    void Update()
    {
        if(impactKillTime != -1 && Time.time >= impactKillTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // impacted
        mesh.SetActive(false);
        for(int i =0; i < impact.Length; i++)
        {
            impact[i].SetActive(true);
        }
        particleSystem.Stop();

        rigidbody.isKinematic = true;

        collider.enabled = false;

        impactKillTime = Time.time + 5;
    }
}
