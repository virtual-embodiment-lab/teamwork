using UnityEngine;
using Normal.Realtime;
using System;
using TMPro;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(UIManager))]
public class Player : MonoBehaviour
{
    [SerializeField] private float walkingSpeed = 7.5f;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 45.0f;
    [SerializeField] public Role currentRole = Role.None;
    [SerializeField] public TMP_Text roleText;
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private Sprite crosshairSprite;
    [SerializeField] private float minWalkingSpeed = 1.0f;
    [SerializeField] private float maxEnergy = 100f;

    private RealtimeView realtimeView;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private bool canMove = true;
    private float currentEnergy;
    private bool isMoving = false;

    public float CurrentEnergy => currentEnergy; // Expose current energy for the UI
    public float MaxEnergy => maxEnergy; // Expose max energy for the UI
    public int Batteries { get; private set; } = MaxBatteries; // Expose batteries for the UI
    public const int MaxBatteries = 25;

    private UIManager uiManager; // Reference to the UIManager

    private void Start()
    {
        realtimeView = GetComponent<RealtimeView>();
        characterController = GetComponent<CharacterController>();
        uiManager = GetComponent<UIManager>();

        if (realtimeView.isOwnedLocallyInHierarchy)
        {
            InitializePlayer();
            uiManager.Initialize(this, playerCamera, roleText, crosshairSprite);
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
        }
    }

    private void InitializePlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetRole(Role.None);
        GetComponent<RealtimeTransform>().RequestOwnership();
        currentEnergy = maxEnergy;
    }

    void Update()
    {
        if (!realtimeView.isOwnedLocallyInHierarchy) return;

        HandleInput();
        HandleMovement();
        HandleRotation();
        uiManager.UpdateUI();

        switch (currentRole)
        {
            case Role.Collector:
                HandleBatteryDrop();
                break;
            case Role.Explorer:
                HandleEnergyConsumption();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!realtimeView.isOwnedLocallyInHierarchy) return;

        switch (currentRole)
        {
            case Role.Tactical:
                TriggerTactical(other);
                break;
            case Role.Explorer:
                PickUpBattery(other);
                break;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TacticalControlTrigger"))
        {
            other.GetComponent<TacticalControlTrigger>().tacticalControl.DisableTacticalControl();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
            uiManager.SetCrosshairVisibility(Cursor.lockState == CursorLockMode.Locked);
        }
    }


    private void HandleMovement()
    {
        if (!canMove) return;

        float curSpeedX, curSpeedY;

        if (currentRole.Equals(Role.Explorer))
        {
            float energyRatio = currentEnergy / maxEnergy;
            float scaledSpeed = Mathf.Lerp(minWalkingSpeed, walkingSpeed, energyRatio);

            curSpeedX = scaledSpeed * Input.GetAxis("Vertical");
            curSpeedY = scaledSpeed * Input.GetAxis("Horizontal");
        } else
        {
            curSpeedX = walkingSpeed * Input.GetAxis("Vertical");
            curSpeedY = walkingSpeed * Input.GetAxis("Horizontal");
        }
        
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


    private void TriggerTactical(Collider other)
    {
        if (other.CompareTag("TacticalControlTrigger") && currentRole.Equals(Role.Tactical))
        {
            other.GetComponent<TacticalControlTrigger>().tacticalControl.AssignPlayerComponents(this, playerCamera);
            other.GetComponent<TacticalControlTrigger>().tacticalControl.EnableTacticalControl();
        }
    }

    private void PickUpBattery(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            // Assuming batteries restore a fixed amount of energy
            float energyRestored = 25f; // Adjust this value as needed
            currentEnergy = Mathf.Min(currentEnergy + energyRestored, maxEnergy);

            // Assuming the battery should be destroyed after being picked up
            Destroy(other.gameObject);

            Debug.Log("Picked up a battery. Energy restored.");
        }
    }

    private void HandleEnergyConsumption()
    {
        // Determine if the player is moving
        isMoving = characterController.velocity.magnitude > 0;

        // If moving, deplete energy
        if (isMoving)
        {
            float energyRatio = currentEnergy / maxEnergy;
            float scaledSpeed = Mathf.Lerp(minWalkingSpeed, walkingSpeed, energyRatio);

            currentEnergy = Mathf.Max(currentEnergy - Time.deltaTime * scaledSpeed, 1); // Keep energy above 0 to avoid division by zero
        }

        // Handle energy reaching zero if needed
        if (currentEnergy <= 1)
        {
            // Perform any logic for when energy depletes (like disabling movement)
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

    public void SetRole(Role newRole)
    {
        if (currentRole != newRole)
        {
            currentRole = newRole;
            uiManager.UpdateRoleUI(currentRole);
            uiManager.SetEnergyBarVisibility(newRole == Role.Explorer);
            if (newRole == Role.Explorer)
            {
                currentEnergy = maxEnergy;
            }
        }
    }

    public string GetFormattedGameTime()
    {
         return DateTime.UtcNow.ToString("mm:ss");
    }

    public int GetCoinsCollected()
    {
        return 0;
    }

    //private void UpdateBatteryRecharge()
    //{
    //    if (currentRole == Role.Collector)
    //    {
    //        batteryTimer += Time.deltaTime;
    //        if (batteryTimer >= batteryRechargeTime)
    //        {
    //            batteries = Mathf.Min(batteries + 1, MaxBatteries);
    //            batteryTimer = 0;
    //            if (currentRole == Role.Collector)
    //                batteryCountText.text = $"Batteries: {batteries}/{MaxBatteries}";
    //        }
    //    }
    //}
}
