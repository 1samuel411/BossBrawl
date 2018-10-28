using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    public GameObject target;
    private PlayerController playerController;
    public float rotateSpeed = 5;
    public Vector3 offsetPos;

    void Start()
    {
        playerController = target.GetComponent<PlayerController>();
    }

    void LateUpdate()
    {
        if (Time.timeScale <= 0)
            return;

        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        transform.eulerAngles += new Vector3(0, horizontal, 0) * Time.deltaTime;

        transform.position = target.transform.position + offsetPos;

        if (playerController.isGrounded && playerController.isMoving)
            target.transform.eulerAngles = new Vector3(target.transform.eulerAngles.x, transform.eulerAngles.y, target.transform.eulerAngles.z);
    }
}
