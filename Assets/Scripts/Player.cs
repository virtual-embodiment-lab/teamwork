using UnityEngine;
using TMPro;
using Normal.Realtime;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField] public float walkingSpeed = 7.5f;
    [SerializeField] public float gravity = 20.0f;
    [SerializeField] public Camera playerCamera;
    [SerializeField] public float lookSpeed = 2.0f;
    [SerializeField] public float lookXLimit = 45.0f;
    [SerializeField] public TMP_Text roleText;
    [SerializeField] public Role currentRole = Role.None;
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private RealtimeView _realtimeView;

    [HideInInspector] private CharacterController characterController;
    [HideInInspector] private Vector3 moveDirection = Vector3.zero;
    [HideInInspector] private float rotationX = 0;
    [HideInInspector] public bool canMove = true;

    private void Awake()
    {
        // Store a reference to the RealtimeView for easy access
        _realtimeView = GetComponent<RealtimeView>();
    }

    void Start()
    {
        if (_realtimeView.isOwnedLocallyInHierarchy)
        {
            LocalStart();
        } else
        {
            playerCamera.gameObject.SetActive(false);
        }
    }

    private void LocalStart()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetRole(Role.None);
        // Request ownership of the Player and the character RealtimeTransforms
        GetComponent<RealtimeTransform>().RequestOwnership();
    }

    void Update()
    {
        if (_realtimeView.isOwnedLocallyInHierarchy)
        {
            HandleInput();
            HandleMovement();
            HandleRotation();
            HandleBatteryDrop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_realtimeView.isOwnedLocallyInHierarchy)
        {
            triggerTactical(other);
            PickUpBattery(other);
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }
    }

    private void HandleMovement()
    {
        if (!canMove) return;

        float curSpeedX = walkingSpeed * Input.GetAxis("Vertical");
        float curSpeedY = walkingSpeed * Input.GetAxis("Horizontal");

        if (characterController.isGrounded)
        {
            moveDirection = transform.TransformDirection(Vector3.forward * curSpeedX + Vector3.right * curSpeedY);
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (canMove)
        {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.Rotate(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void HandleBatteryDrop()
    {
        if (currentRole == Role.Collector && Input.GetKeyDown(KeyCode.B))
        {
            // Spawn the battery a bit in front of the player
            Vector3 spawnPosition = transform.position + transform.forward * 1.5f; // Adjust the multiplier as needed for the desired distance
            Instantiate(batteryPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void PickUpBattery(Collider other)
    {
        if (other.CompareTag("Battery") && currentRole.Equals(Role.Explorer))
        {
            Debug.Log("get Battery");
        }
    }

    public void SetRole(Role newRole)
    {
        if (currentRole != newRole)
        {
            currentRole = newRole;
            UpdateRoleText();
        }
    }

    private void UpdateRoleText()
    {
        if (roleText != null)
        {
            roleText.text = currentRole.ToString();
        }
    }

    private void triggerTactical(Collider other)
    {
        if (other.CompareTag("TacticalControlTrigger") && currentRole.Equals(Role.Tactical))
        {
            other.GetComponent<TacticalControlTrigger>().tacticalControl.AssignPlayerComponents(this, playerCamera);
            other.GetComponent<TacticalControlTrigger>().tacticalControl.EnableTacticalControl();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the collider they exit has the tag "TacticalControlTrigger"
        if (other.CompareTag("TacticalControlTrigger"))
        {
            other.GetComponent<TacticalControlTrigger>().tacticalControl.DisableTacticalControl();
        }
    }
}
