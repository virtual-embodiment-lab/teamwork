#if NORMCORE

using UnityEngine;
using UnityEngine.XR;
using Normal.Realtime;

public class SimpleVRMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 100.0f;

    // Define XRNodes for the left and right hand controllers
    public XRNode leftHandController = XRNode.LeftHand;
    public XRNode rightHandController = XRNode.RightHand;

    private RealtimeView _realtimeView;
    private RealtimeTransform _realtimeTransform;

    private void Awake()
    {
        _realtimeView = GetComponent<RealtimeView>();
        _realtimeTransform = GetComponent<RealtimeTransform>();
    }

    private void Update()
    {
        if (!_realtimeView.isOwnedLocallySelf)
            return;

        _realtimeTransform.RequestOwnership();

        // Get joystick inputs
        Vector2 leftJoystickInput = GetJoystickInput(leftHandController);
        Vector2 rightJoystickInput = GetJoystickInput(rightHandController);

        // Handle rotation and movement
        HandleRotation(leftJoystickInput.x);
        HandleMovement(rightJoystickInput);

        // Log joystick values for debugging
        Debug.Log($"Left Joystick X (Rotation): {leftJoystickInput.x}");
        Debug.Log($"Right Joystick (Movement): {rightJoystickInput}");
    }

    Vector2 GetJoystickInput(XRNode controller)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controller);
        Vector2 inputAxis;

        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis))
            return inputAxis;

        return Vector2.zero;
    }

    void HandleMovement(Vector2 inputAxis)
    {
        Vector3 direction = new Vector3(inputAxis.x, 0, inputAxis.y) * speed * Time.deltaTime;
        transform.localPosition += direction;
    }

    void HandleRotation(float inputX)
    {
        // Rotate around the y-axis
        transform.Rotate(0, inputX * rotationSpeed * Time.deltaTime, 0);
    }
}

#endif
