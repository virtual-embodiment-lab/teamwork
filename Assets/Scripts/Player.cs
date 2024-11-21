// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using UnityEngine;
using Normal.Realtime;
using TMPro;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(UIManager))]
public class Player : RealtimeComponent<PlayerModel>
{
    [SerializeField] public CoinShape targetCoin = CoinShape.None;
    [SerializeField] private float walkingSpeed = 7.5f;
    [SerializeField] private float gravity = 20.0f;
    //[SerializeField] private Camera playerCamera;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 45.0f;
    [SerializeField] public Role currentRole = Role.None;
    [SerializeField] public TMP_Text roleText;
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private Sprite crosshairSprite;
    [SerializeField] private float minWalkingSpeed = 1.0f;
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float batteryConsumption = 33f;
    [SerializeField] private float batteryRechargeTime = 2.0f;
    [SerializeField] public string layerToActivate = "Collector";

    private float batteryTimer = 0f;
    private RealtimeView realtimeView;
    private GameManager gameManager;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private bool canMove = true;
    private float currentEnergy;
    private bool isMoving = false;
    private bool isTacticalModeActive = false;

    public float CurrentEnergy => currentEnergy; // Expose current energy for the UI
    public float MaxEnergy => maxEnergy; // Expose max energy for the UI
    public int carryingBatteries { get; private set; } = 0; // Expose batteries for the UI
    public int carryingCoins { get; private set; } = 0;
    public int gamePhase { get; private set; } = 0; // 0 = tutorial, 1 = playing, 2 = timeout

    [SerializeField] public const int MaxBatteries = 1;

    private UIManager uiManager; // Reference to the UIManager
    private Logger_new lg;


    private void Start()
    {
        // roleText = GetComponentInChildren<TMP_Text>(); // 11/13
        realtimeView = GetComponent<RealtimeView>();
        characterController = GetComponent<CharacterController>();
        gameManager = FindObjectOfType<GameManager>();
        uiManager = GetComponent<UIManager>();
        roleText = GetComponentInChildren<TMP_Text>();
        lg = GetComponent<Logger_new>();

        if (realtimeView.isOwnedLocallyInHierarchy)
        {
            InitializePlayer();
            uiManager.Initialize(this, crosshairSprite, gameManager);
        }
        else
        {
            //playerCamera.gameObject.SetActive(false);
        }
    }

    protected override void OnRealtimeModelReplaced(PlayerModel previousModel, PlayerModel currentModel)
    {
        if (previousModel != null){
            previousModel.roleDidChange -= RoleDidChange;
        }

        if (currentModel != null){
            currentModel.roleDidChange += RoleDidChange;
            if (currentModel.isFreshModel) {
                currentModel.role = 0;
            }
        }
    }

    public int GetRole() {
        return (int)model.role;
    }
    
    public void EndTrial()
    {
        if (uiManager == null)
        {
            Debug.LogError("UIManager is null in EndTrial");
            return;
        }
        canMove = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gamePhase = 2;
        uiManager.DisplayTrialOverScreen();
    }


    private void InitializePlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetRole(Role.None);
        GetComponent<RealtimeTransform>().RequestOwnership();
        currentEnergy = maxEnergy;
    }

    public void GetStarted()
    {
        gamePhase = 1;
    }

    void Update()
    {
        if (!realtimeView.isOwnedLocallyInHierarchy) return;
        AudioListener audioListener = FindObjectOfType<AudioListener>();

        UpdatePlayerModels();

        HandleInput();
        //HandleMovement();
        //HandleRotation();
        uiManager.UpdateUI();

        switch (currentRole)
        {
            case Role.Collector:
                //UpdateBatteryRecharge();
                SetCollectorVisibility();
                HandleBatteryDrop();
                break;
            case Role.Explorer:
                //HandleEnergyConsumption();
                HideCollectorLayer();
                break;
            case Role.Tactical:
                HideCollectorLayer();
                break;
        }
        
    }

    private void UpdatePlayerModels()
    {
        if (model.role != (int)currentRole)
        {
            SetRole((Role)model.role);
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
            case Role.Collector:
                boxEvents(other);
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TacticalControlTrigger"))
        {
            other.GetComponent<TacticalControlTrigger>().tacticalControl.IsTacticalModeActive = false;
            isTacticalModeActive = false;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isTacticalModeActive)
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }
    }

    private void HandleMovement()
    {
        if (!canMove) return;

        float curSpeedX, curSpeedY;

        if (currentRole.Equals(Role.Explorer))
        {
            // does not decrease walking speed as long as energy is not gone.
            float energyRatio = 1;
            if (currentEnergy <= 1)
            {
                energyRatio = 1.0f / maxEnergy;
            }

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
            transform.Rotate(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void TriggerTactical(Collider other)
    {
        if (other.CompareTag("TacticalControlTrigger") && currentRole.Equals(Role.Tactical))
        {
            other.GetComponent<TacticalControlTrigger>().tacticalControl.IsTacticalModeActive = true;
            isTacticalModeActive = true;
            uiManager.UpdateRoleDependentUI();
        }
    }

    private void PickUpBattery(Collider other)
    {
        if (other.CompareTag("Battery") || (other.gameObject.name == "batteryBox"))
        {
            // Assuming batteries restore a fixed amount of energy
            // float energyRestored = 10f*maxEnergy/20f; // Adds 10 secs. Adjust this value as needed
            // currentEnergy = Mathf.Min(currentEnergy + energyRestored, maxEnergy);
            currentEnergy = maxEnergy;
            lg.AddLine("Battery:pickUp");
            
            if (other.CompareTag("Battery"))
            {
                // Assuming the battery should be destroyed after being picked up
                Destroy(other.gameObject);
            }

            Debug.Log("Picked up a battery. Energy restored.");
        }
    }

    public void HandleEnergyConsumption(Player player)
    {
        player.currentEnergy -= batteryConsumption;
    }

    private void boxEvents(Collider other)
    {
        if (other.gameObject.name == "coinBox")
        {
            carryingCoins = 0;

            OVRPlayerController playerCon = GameObject.Find("OVRPlayerController").GetComponent<OVRPlayerController>();
            float carryingLoad = (0.1f * 0.15f * carryingCoins) + (0.1f * 0.03f * carryingBatteries); //10% a coin, 3% a battery
            playerCon.Acceleration = 0.1f - carryingLoad;

            lg.AddLine("dropCoins");
            lg.AddLine("carryingCoins:0");
        }
        else if (other.gameObject.name == "batteryBox")
        {
            if (carryingBatteries < MaxBatteries)
            {
                carryingBatteries ++;

                OVRPlayerController playerCon = GameObject.Find("OVRPlayerController").GetComponent<OVRPlayerController>();
                float carryingLoad = (0.1f * 0.15f * carryingCoins) + (0.1f * 0.03f * carryingBatteries); //10% a coin, 3% a battery
                playerCon.Acceleration = 0.1f - carryingLoad;

                lg.AddLine("pickUpBattery");
                string line = $"carryingBatteries:{carryingBatteries}";
                lg.AddLine(line);
            }
        }
    }

    private void HandleBatteryDrop()
    {
        if (currentRole == Role.Collector && (Input.GetKey(KeyCode.B) || OVRInput.GetUp(OVRInput.RawButton.X)) && carryingBatteries >= 1)
        {
            carryingBatteries--;
            lg.AddLine("Battery:drop");
            Vector3 spawnPosition = transform.position + transform.forward * 1.5f;
            spawnPosition.y += 0.8f;
            Realtime.Instantiate(batteryPrefab.name, spawnPosition, Quaternion.identity, new Realtime.InstantiateOptions { });

            OVRPlayerController playerCon = GameObject.Find("OVRPlayerController").GetComponent<OVRPlayerController>();
            float carryingLoad = (0.1f * 0.15f * carryingCoins) + (0.1f * 0.03f * carryingBatteries); //10% a coin, 3% a battery
            playerCon.Acceleration = 0.1f - carryingLoad;
        }
    }

    public void SetRole(Role newRole)
    {
        if (currentRole != newRole)
        {
            RoleDidChange(model, (int)newRole);
            currentRole = newRole;
            uiManager.UpdateRoleUI(currentRole);
            uiManager.SetEnergyBarVisibility(newRole == Role.Explorer);
            if (newRole == Role.Explorer)
            {
                currentEnergy = maxEnergy;
            }
        }
    }

    private void RoleDidChange(PlayerModel model, int value) {
        model.role = value;
        if (value == 0) {
            roleText.text = "No Role";
        } else if (value == 1) {
            roleText.text = "Collector";
        } else if (value == 2) {
            roleText.text = "Tactical";
        } else {
            roleText.text = "Explorer";
        }
        
    }

    public void collectCoin()
    {
        carryingCoins ++;
        OVRPlayerController playerCon = GameObject.Find("OVRPlayerController").GetComponent<OVRPlayerController>();
        float carryingLoad = (0.1f * 0.15f * carryingCoins) + (0.1f * 0.03f * carryingBatteries); //10% a coin, 3% a battery
        playerCon.Acceleration = 0.1f - carryingLoad;
    }
    
    public string GetFormattedGameTime()
    {
        return gameManager != null ? gameManager.GetFormattedGameTime() : "00:00";
    }


    public int GetCoinsCollected()
    {
        return 0;
    }

    public void SetCollectorVisibility()
    {
        /*
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
                return;
        }
        int layerNumber = LayerMask.NameToLayer(layerToActivate);
        if (layerNumber == -1)
        {
            Debug.LogError($"Layer '{layerToActivate}' not found.");
            return;
        }
        int layerMask = 1 << layerNumber;
        playerCamera.cullingMask |= layerMask;
        */
    }

    public void HideCollectorLayer()
    {
        /*
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
                return;
        }

        int layerNumber = LayerMask.NameToLayer(layerToActivate);
        if (layerNumber == -1)
            return;
        int layerMask = 1 << layerNumber;
        playerCamera.cullingMask &= ~layerMask;
        */
    }


    private void UpdateBatteryRecharge()
    {
        if (carryingBatteries < MaxBatteries)
        {
            batteryTimer += Time.deltaTime;
            if (batteryTimer >= batteryRechargeTime)
            {
                carryingBatteries++;
                batteryTimer = 0;
                uiManager.UpdateBatteryNumber();
            }
        }
    }

}
