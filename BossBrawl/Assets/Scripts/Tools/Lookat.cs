using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lookat : MonoBehaviour
{

    public Transform target;
    public Vector2 clamp;

    void Start()
    {

    }

    void LateUpdate()
    {
        transform.LookAt(target);
        Vector3 rot = transform.localEulerAngles;
        rot.y = Mathf.Clamp(rot.y, clamp.x, clamp.y);
        transform.localEulerAngles = rot;
    }
}
