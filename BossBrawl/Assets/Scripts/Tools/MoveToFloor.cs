using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToFloor : MonoBehaviour
{

    public LayerMask mask;

    public Vector2 clampX;
    public Vector2 clampZ;

    void Start()
    {

    }

    void LateUpdate()
    {
        // Clamp me
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, clampX.x, clampX.y);
        pos.z = Mathf.Clamp(pos.z, clampZ.x, clampZ.y);
        transform.position = pos;

        // Create ray
        RaycastHit hit;
        if(Physics.Raycast(transform.position + Vector3.up * 100, Vector3.down, out hit, 10000, mask, QueryTriggerInteraction.UseGlobal))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
    }
}
