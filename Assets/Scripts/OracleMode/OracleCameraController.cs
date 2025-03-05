using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class OracleCameraController : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float flySpeed = 100.0f;
    public float yawSpeed = 10.0f;
    public float pitchSpeed = 10.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private int LEFT_CLICK = 0;

    void Start()
    {

    }

    void Update()
    {
        // left click and move mouse to rotate
        if (Input.GetMouseButton(LEFT_CLICK))
        {
            yaw += yawSpeed * Input.GetAxis("Mouse X");
            pitch -= pitchSpeed * Input.GetAxis("Mouse Y");
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
        // WASD keys to move in XZ plane
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float curSpeedX = moveSpeed * Input.GetAxis("Vertical");
        float curSpeedZ = moveSpeed * Input.GetAxis("Horizontal");
        Vector3 moveDirection = (forward * curSpeedX) + (right * curSpeedZ);

        // QE keys to move along Y axis (go up / down)
        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 tempVect = new Vector3(0, -1, 0);
            tempVect = tempVect.normalized * flySpeed * Time.deltaTime;
            moveDirection += tempVect;
        }
        if (Input.GetKey(KeyCode.E))
        {
            Vector3 tempVect = new Vector3(0, 1, 0);
            tempVect = tempVect.normalized * flySpeed * Time.deltaTime;
            moveDirection += tempVect;
        }
        // Move the controller
        transform.position += moveDirection * Time.deltaTime;
    }

}
