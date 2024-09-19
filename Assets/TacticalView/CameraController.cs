// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float speed = 5.0f; //test speed for most participants 
    public float sensitivity = 5.0f;
    public bool enableControl = false;

    public InputActionReference YButtonAction;
    public InputActionReference rightJoystickAction;
    public InputActionReference leftJoystickAction;

    void Awake()
    {
        YButtonAction.action.performed += OnYButtonPressed;
        YButtonAction.action.Enable();
    }

    void onDestroy()
    {
        YButtonAction.action.performed -= OnYButtonPressed;
        YButtonAction.action.Disable();
    }

    private void OnYButtonPressed(InputAction.CallbackContext context)
    {
        GameObject switcher = GameObject.Find("switcher");
        switcher.GetComponent<contorllerSwitcher>().switchMode(false);
        enableControl = false;
    }

    void Update()
    {
        if (enableControl == true){

            // get left joystick angle
            Vector2 leftJoystic = leftJoystickAction.action.ReadValue<Vector2>();
            float horizontalL = leftJoystic.x;
            float verticalL = leftJoystic.y;

            // get right joystick angle
           // float horizontalR = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
            Vector2 rightJoystic = rightJoystickAction.action.ReadValue<Vector2>();
            float verticalR = rightJoystic.y;

            transform.position += transform.up * -verticalL * speed * Time.deltaTime;
            transform.position += transform.right * -horizontalL * speed * Time.deltaTime;

            // Move camera up/down (height) using the right joystick
            transform.position += transform.forward * -verticalR * speed * Time.deltaTime;
        }
    }

   
}


