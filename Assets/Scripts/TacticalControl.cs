using UnityEngine;

public class TacticalControl : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform pointer;
    [SerializeField] private GameObject tacticalCamera;

    private Vector3 velocity = Vector3.zero;
    private Rigidbody rb;

    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float scrollSpeed = 20f;
    [SerializeField] private Vector2 panLimitX;
    [SerializeField] private Vector2 panLimitZ;
    [SerializeField] private float minY = 5f;
    [SerializeField] private float maxY = 30f;
    [SerializeField] private float smoothTime = 0.5f;

    // The player's control script and camera
    [SerializeField] private MonoBehaviour playerControlScript;
    [SerializeField] private GameObject playerCamera;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Disable the tactical controls at the start
        tacticalCamera.SetActive(false);
        enabled = false; // Ensure this script is disabled
    }

    void Update()
    {
        if (!enabled) return;

        float deltaTime = Time.deltaTime;
        Vector3 pos = pointer.transform.position;

        // Combine input checks to reduce duplication
        pos.x += (Input.GetKey("s") || Input.GetKey("down") ? 1 : Input.GetKey("w") || Input.GetKey("up") ? -1 : 0) * panSpeed * deltaTime;
        pos.z += (Input.GetKey("d") || Input.GetKey("right") ? 1 : Input.GetKey("a") || Input.GetKey("left") ? -1 : 0) * panSpeed * deltaTime;

        // Simplify the scroll and elevation control
        float scroll = -Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * 100f * deltaTime;
        scroll += (Input.GetKey("e") ? 1 : Input.GetKey("q") ? -1 : 0) * scrollSpeed * 100f * deltaTime;
        pos.y += scroll;

        //pos.x = Mathf.Clamp(pos.x, panLimitX.x, panLimitX.y);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        //pos.z = Mathf.Clamp(pos.z, panLimitZ.x, panLimitZ.y);
        pointer.transform.position = pos;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(velocity) * Time.fixedDeltaTime);
    }

    public void AssignPlayerComponents(MonoBehaviour controlScript, Camera camera)
    {
        playerControlScript = controlScript;
        playerCamera = camera.gameObject;
    }

    public void EnableTacticalControl()
    {
        if (playerControlScript != null)
        {
            playerControlScript.enabled = false;
        }

        if (playerCamera != null)
        {
            playerCamera.SetActive(false);
        }

        tacticalCamera.SetActive(true);
        enabled = true; // Enable this TacticalControl script
    }

    public void DisableTacticalControl()
    {
        if (playerControlScript != null)
        {
            playerControlScript.enabled = true;
        }

        if (playerCamera != null)
        {
            playerCamera.SetActive(true);
        }

        tacticalCamera.SetActive(false);
        enabled = false; // Disable this TacticalControl script
    }
}
