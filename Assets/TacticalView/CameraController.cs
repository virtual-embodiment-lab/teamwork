using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 5.0f; //test speed for most participants 
    public float sensitivity = 5.0f;
    public bool enableControl = false;

    // Start is called before the first frame update
    void Start()
    {
    
    }

   
    void Update()
    {
        if (enableControl == true){
            if (OVRInput.GetUp(OVRInput.RawButton.Y))
            {
                GameObject switcher = GameObject.Find("switcher");
                switcher.GetComponent<contorllerSwitcher>().switchMode(false); 
                enableControl = false;               
            }

            // get left joystick angle
            float horizontalL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
            float verticalL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;

            // get right joystick angle
            float horizontalR = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
            float verticalR = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;

/*
            if ( Input.GetKey(KeyCode.UpArrow) )
                dir_v = 1;
            if ( Input.GetKey(KeyCode.DownArrow) )
                dir_v = -1;
            if ( Input.GetKey(KeyCode.RightArrow) )
                dir_h = 1;
            if ( Input.GetKey(KeyCode.LeftArrow) )
                dir_h = -1;
*/
            //move camera positoin
            transform.position += transform.right * -horizontalL * speed * Time.deltaTime;
            transform.position += transform.up * -verticalL * speed * Time.deltaTime;

            //move camera height
            transform.position += transform.forward * verticalR * speed * Time.deltaTime;
        }

        //add invisible block trigger in the room + collision information of avatar to get the role of avator and switch mode if tactical
        //let mingyi know where trigger is stored
        //invisible object near starting room that the camera detects to go back to normal mode add duration to go back to normal mode or mnitor mode
    }
}


