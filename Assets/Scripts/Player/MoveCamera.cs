using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    public float mouseSpeed = 100f;
    public Transform playerBody;
    public Transform target;
    public float maxY = 90f;
    public float minY = -90f;
    float xRotation = 0f;

    void Start()
    {
       Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;


        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minY, maxY);

        //transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
        target.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}