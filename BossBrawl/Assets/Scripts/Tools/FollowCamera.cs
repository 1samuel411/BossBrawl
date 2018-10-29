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
        transform.localPosition = Vector3.zero;
        if (Time.timeScale <= 0)
            return;

        float horizontal = InputManager.instance.player2.GetAxis("XLook") * rotateSpeed;
        //float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        float vertical = InputManager.instance.player2.GetAxis("YLook") * -rotateSpeed * 0.9f;
        //float vertical = Input.GetAxis("Mouse Y") * -rotateSpeed * 0.9f;
        Vector3 difference = new Vector3(vertical, horizontal, 0) * Time.deltaTime;
        transform.Rotate(difference);

        Vector3 rot = transform.localEulerAngles;
        rot.z = 0;
        transform.localEulerAngles = rot;

        transform.position = target.transform.position + offsetPos;

        if ((playerController.isGrounded && playerController.isMoving) || playerController.attacking)
            target.transform.eulerAngles = new Vector3(target.transform.eulerAngles.x, transform.eulerAngles.y, target.transform.eulerAngles.z);
    }
}
