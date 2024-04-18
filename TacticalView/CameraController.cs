using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float speed = 5.0f; //test speed for most participants 
    public float sensitivity = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
    
    }

   
    void Update()
    {
        //// Move the camera forward, backward, left, and right
        //transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        //transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
   
        //// Move the camera forward, backward, left, and right
        //transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        //transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        //// Rotate the camera based on the mouse movement
        //float mouseX = Input.GetAxis("Mouse X");
        //float mouseY = Input.GetAxis("Mouse Y"); 
        //transform.eulerAngles += new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);


        // Move the camera forward, backward, left, and right
        transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        // Move the camera forward, backward, left, and right
        transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        // Rotate the camera based on the joystick movement
        float pressX = Input.GetAxis("Joy0X"); //joystick 1 
        float pressY = Input.GetAxis("Joy1Y"); //joystick 2
        transform.eulerAngles += new Vector3(-pressY * sensitivity, pressX * sensitivity, 0);

        //add invisible block trigger in the room + collision information of avatar to get the role of avator and switch mode if tactical
        //let mingyi know where trigger is stored
        //invisible object near starting room that the camera detects to go back to normal mode add duration to go back to normal mode or mnitor mode
    }
}


