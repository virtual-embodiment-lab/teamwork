// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 5.0f; //test speed for most participants 
    public float sensitivity = 5.0f;
    public bool enableControl = false;

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

            //move camera positoin
            transform.position += transform.right * -horizontalL * speed * Time.deltaTime;
            transform.position += transform.up * -verticalL * speed * Time.deltaTime;

            //move camera height
            transform.position += transform.forward * verticalR * speed * Time.deltaTime;
        }
    }
}


