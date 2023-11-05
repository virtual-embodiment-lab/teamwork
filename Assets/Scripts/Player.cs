using UnityEngine;
using TMPro;
using Normal.Realtime;
using UnityEngine.UI;

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
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Sprite crosshairSprite;

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
        CreateCrosshairUI();
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

            if (crosshairImage != null)
                crosshairImage.enabled = Cursor.lockState == CursorLockMode.Locked;
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
            _ = Realtime.Instantiate(batteryPrefab.name, spawnPosition, Quaternion.identity, new Realtime.InstantiateOptions { });
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

    private void CreateCrosshairUI()
    {
        // Create a new Canvas object as a child of the camera
        GameObject canvasObject = new GameObject("CrosshairCanvas");
        canvasObject.transform.SetParent(playerCamera.transform, false);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();

        // Set the canvas to cover the whole camera view
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(playerCamera.pixelWidth, playerCamera.pixelHeight);
        canvasRect.localPosition = new Vector3(0, 0, 0);

        // Create the Image component for the crosshair
        GameObject crosshairObject = new GameObject("Crosshair");
        crosshairImage = crosshairObject.AddComponent<Image>();
        crosshairImage.sprite = crosshairSprite;
        crosshairImage.rectTransform.sizeDelta = new Vector2(25, 25); // Set the size of the crosshair here

        // Set the crosshair to be at the center of the screen
        crosshairImage.rectTransform.SetParent(canvas.transform, false);
        crosshairImage.rectTransform.anchoredPosition = Vector2.zero;

        // Enable the crosshair by default
        crosshairImage.enabled = true;
    }
}
