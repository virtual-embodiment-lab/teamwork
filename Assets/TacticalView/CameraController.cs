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

    // Start is called before the first frame update
    void Start()
    {
    
    }

   
    void Update()
    {
        if (enableControl == true){
/* 
            if (OVRInput.GetUp(OVRInput.RawButton.Y))
            {
                GameObject switcher = GameObject.Find("switcher");
                switcher.GetComponent<contorllerSwitcher>().switchMode(false); 
                enableControl = false;               
            }
 */
            // get left joystick angle
            Vector2 leftJoystic = leftJoystickAction.action.ReadValue<Vector2>();
            float horizontalL = leftJoystic.x;
            float verticalL = leftJoystic.y;

            // get right joystick angle
           // float horizontalR = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
            Vector2 rightJoystic = rightJoystickAction.action.ReadValue<Vector2>();
            float verticalR = rightJoystic.y;

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
            //transform.position += transform.right * -horizontalL * speed * Time.deltaTime;
            //transform.position += transform.up * -verticalL * speed * Time.deltaTime;

            //move camera height
            //transform.position += transform.forward * verticalR * speed * Time.deltaTime;

            //added new lines for camera movement and axis: //
            // Move camera forward/backward and left/right using the left joystick
            transform.position += transform.up * -verticalL * speed * Time.deltaTime;
            transform.position += transform.right * -horizontalL * speed * Time.deltaTime;

            // Move camera up/down (height) using the right joystick
            transform.position += transform.forward * -verticalR * speed * Time.deltaTime;
        }

        //add invisible block trigger in the room + collision information of avatar to get the role of avator and switch mode if tactical
        //let mingyi know where trigger is stored
        //invisible object near starting room that the camera detects to go back to normal mode add duration to go back to normal mode or mnitor mode
    }

   
}


