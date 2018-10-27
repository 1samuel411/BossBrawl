using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCamera : MonoBehaviour
{

    public Transform target;

    public float moveSpeed, rotateSpeed;

    void Start()
    {

    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotateSpeed * Time.deltaTime);
    }
}
