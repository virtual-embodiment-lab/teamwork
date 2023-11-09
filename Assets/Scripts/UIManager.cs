using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager: MonoBehaviour
{
    private Player _player;
    private Sprite _crosshairSprite;

    private TacticalControl _tacticalControl;
    private GameManager _gameManager;
    private Image _roleColorIndicator;
    private Image _uiPanelBackground;
    private Image _crosshairImage;
    private Image _energyBar;
    private TextMeshProUGUI _gameTimeText;
    private TextMeshProUGUI _roleUIText;
    private TextMeshProUGUI _coinsCollectedText;
    private TextMeshProUGUI _batteryCountText;
    private Button _exitTacticalButton;
    private RectTransform _uiPanel;
    private Canvas _mainCanvas;

    public void Initialize(Player player, Sprite crosshairSprite, GameManager gameManager)
    {
        _player = player;
        _crosshairSprite = crosshairSprite;
        _gameManager = gameManager;
        _tacticalControl = FindObjectOfType<TacticalControl>();

        InitializeMainCanvas();
        CreateCrosshairUI();
        CreateMainUIPanel();
        CreateGameTimeUI();
        CreateRoleUI();
        CreateRoleColorIndicator();
        CreateCoinsCollectedUI();
        CreateBatteryCountUI();
        CreateEnergyBarUI();
        CreateExitTacticalButton();
        UpdateEnergyUI();
    }

    public void UpdateUI()
    {
        UpdateGameTimeUI();
        UpdateCoinsCollectedUI();
        UpdateBatteryRecharge();
        UpdateEnergyUI();
        UpdateRoleDependentUI();
    }

    private void InitializeMainCanvas()
    {
        GameObject canvasObject = new GameObject("Canvas");
        _mainCanvas = canvasObject.AddComponent<Canvas>();
        _mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();
        canvasObject.transform.SetParent(transform, false);
    }

    private void CreateCrosshairUI()
    {
        GameObject crosshairObject = new GameObject("Crosshair");
        _crosshairImage = crosshairObject.AddComponent<Image>();
        _crosshairImage.sprite = _crosshairSprite;
        _crosshairImage.rectTransform.sizeDelta = new Vector2(25, 25); // Set the size of the crosshair here

        // Set the crosshair to be at the center of the screen
        _crosshairImage.rectTransform.SetParent(_mainCanvas.transform, false);
        _crosshairImage.rectTransform.anchoredPosition = Vector2.zero;

        // Enable the crosshair by default
        _crosshairImage.enabled = true;
    }

    private void CreateMainUIPanel()
    {
        GameObject panelObject = new GameObject("UIPanel");
        _uiPanel = panelObject.AddComponent<RectTransform>();

        // Set the panel to be at the top-left with a specific width and height
        _uiPanel.anchorMin = new Vector2(0, 1);
        _uiPanel.anchorMax = new Vector2(0, 1);
        _uiPanel.pivot = new Vector2(0, 1);
        _uiPanel.sizeDelta = new Vector2(120, 80);
        _uiPanel.anchoredPosition = new Vector2(10, -10);

        // Add a background image to the UI Panel
        _uiPanelBackground = panelObject.AddComponent<Image>();
        _uiPanelBackground.color = new Color(0, 0, 0, 0.7f);
        _uiPanelBackground.raycastTarget = false;
        _uiPanelBackground.sprite = Resources.Load<Sprite>("rounded_corner");
        _uiPanelBackground.type = Image.Type.Sliced;

        _uiPanel.SetParent(_mainCanvas.transform, false);
    }

    private void CreateGameTimeUI()
    {
        _gameTimeText = CreateUIElement<TextMeshProUGUI>("GameTimeText", new Vector2(0, 1), new Vector2(0.5f, 1), new Vector2(5, -15), new Vector2(-10, -15), 20);
        _gameTimeText.text = "00:00";
    }

    private void CreateRoleUI()
    {
        Vector2 anchorMin = new Vector2(0, 1);
        Vector2 anchorMax = new Vector2(0, 1);

        Vector2 offsetMin = new Vector2(5, -35);
        Vector2 offsetMax = new Vector2(5 + 70, -35 - 10);

        // Create the UI element with the specified values
        _roleUIText = CreateUIElement<TextMeshProUGUI>("RoleText", anchorMin, anchorMax, offsetMin, offsetMax, 20);
        _roleUIText.alignment = TextAlignmentOptions.Left;
        _roleUIText.text = _player.currentRole.ToString();
    }

    private void CreateCoinsCollectedUI()
    {
        _coinsCollectedText = CreateUIElement<TextMeshProUGUI>("CoinsCollectedText", new Vector2(0.5f, 1), new Vector2(1, 1), new Vector2(0, -15), new Vector2(-5, -15), 20);
        _coinsCollectedText.text = "Coins: 0";
    }

    private void CreateBatteryCountUI()
    {
        _batteryCountText = CreateUIElement<TextMeshProUGUI>("BatteryCountText", new Vector2(0.5f, 1), new Vector2(1, 1), new Vector2(-55, -65), new Vector2(-5, -65), 20);
        _batteryCountText.text = $"Batteries: {_player.Batteries}";
    }

    private void CreateExitTacticalButton()
    {
        // Define button size and position similar to the battery counter and energy bar
        Vector2 anchorMin = new Vector2(0, 1);
        Vector2 anchorMax = new Vector2(0, 1);
        Vector2 offsetMin = new Vector2(5, -70); // These values should be adjusted to match your UI layout
        Vector2 offsetMax = new Vector2(115, -60);   // These values should be adjusted to match your UI layout
        float buttonHeight = 20f; // The height of the button

        // Call the CreateTMPButton method to create the button
        _exitTacticalButton = CreateTMPButton("ExitTacticalButton", anchorMin, anchorMax, offsetMin, offsetMax, buttonHeight);

        // Get the TextMeshProUGUI component from the button's child to set the text
        TextMeshProUGUI buttonText = _exitTacticalButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = "Exit Tactical"; // Set the button's text

        _exitTacticalButton.gameObject.SetActive(false);

        // Optionally, you can add an event listener to the button to define what happens when it is clicked
        _exitTacticalButton.onClick.AddListener(OnExitTacticalButtonClicked);
    }

    private void CreateEnergyBarUI()
    {
        GameObject energyBarObject = new GameObject("EnergyBar");
        _energyBar = energyBarObject.AddComponent<Image>();
        _energyBar.color = Color.green;

        RectTransform rt = _energyBar.rectTransform;
        rt.pivot = new Vector2(0, 0.5f); // Pivot set to the left-middle

        float energyBarTop = -55; // Top position below the role text with a gap

        // Assuming the UIPanel is 100 units wide, and we want the energy bar to start 60 units from the left
        float energyBarHorizontalStart = 60; // Start 60 units from the left

        // Set up the RectTransform to start 60 units from the left and be 110 units wide
        SetupUIElement(rt, new Vector2(0, 1), new Vector2(0, 1),
                       new Vector2(energyBarHorizontalStart, energyBarTop),
                       new Vector2(energyBarHorizontalStart, energyBarTop - 20));

        _energyBar.enabled = false; // Initially disabled, can be enabled when needed
        energyBarObject.transform.SetParent(_uiPanel, false); // Set the parent of the energy bar to the UIPanel
    }

    private void CreateRoleColorIndicator()
    {
        GameObject colorIndicatorObject = new GameObject("RoleColorIndicator");
        _roleColorIndicator = colorIndicatorObject.AddComponent<Image>();
        RectTransform rt = _roleColorIndicator.GetComponent<RectTransform>();

        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 0.5f);
        rt.sizeDelta = new Vector2(35, 20);
        rt.anchoredPosition = new Vector2(_roleUIText.rectTransform.anchoredPosition.x + 40, _roleUIText.rectTransform.anchoredPosition.y);

        colorIndicatorObject.transform.SetParent(_uiPanel.transform, false);
        UpdateRoleColorIndicator(_player.currentRole); // Set initial color
    }

    private T CreateUIElement<T>(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, float? height = null) where T : Component
    {
        GameObject uiElementObject = new GameObject(name);
        T uiElement = uiElementObject.AddComponent<T>();
        RectTransform rt = uiElement.GetComponent<RectTransform>();

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        if (height.HasValue)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height.Value);
        }
        rt.localScale = Vector3.one;

        uiElementObject.transform.SetParent(_uiPanel, false);

        // Specific setup for TextMeshProUGUI
        if (typeof(T) == typeof(TextMeshProUGUI))
        {
            var textElement = uiElement as TextMeshProUGUI;
            //textElement.fontSize = 18;
            textElement.color = Color.white;
            textElement.alignment = TextAlignmentOptions.Center;
            textElement.enableAutoSizing = true;
            textElement.fontSizeMin = 0; // Set the minimum size to 0

        }

        return uiElement;
    }

    private Button CreateTMPButton(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, float? height = null)
    {
        // Create the button object
        GameObject buttonObject = new GameObject(name);
        Button button = buttonObject.AddComponent<Button>();
        Image buttonImage = buttonObject.AddComponent<Image>(); // The button also needs an Image component to display the background
        buttonImage.color = Color.white; // Set default button color

        // Set up the RectTransform for the button
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchorMin;
        buttonRect.anchorMax = anchorMax;
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = Vector2.zero;
        buttonRect.offsetMin = offsetMin;
        buttonRect.offsetMax = offsetMax;
        if (height.HasValue)
        {
            buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, height.Value);
        }
        buttonRect.localScale = Vector3.one;
        buttonObject.transform.SetParent(_uiPanel, false);

        // Create the TextMeshPro Text object as a child of the button
        GameObject textObject = new GameObject("Text");
        TextMeshProUGUI buttonText = textObject.AddComponent<TextMeshProUGUI>();
        buttonText.text = "Button Text"; // Replace with your button text
        buttonText.color = Color.black; // Set text color
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.enableAutoSizing = true;
        buttonText.fontSizeMin = 10;
        buttonText.fontSizeMax = 30;

        // Set up the RectTransform for the text
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        textObject.transform.SetParent(buttonRect, false);

        // Set the button click event, you can add your own method to handle the click event
        button.onClick.AddListener(() => Debug.Log("Button clicked!"));

        return button;
    }


    private void SetupUIElement(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        rt.localScale = Vector3.one;
    }

    private void UpdateRoleColorIndicator(Role role)
    {
        switch (role)
        {
            case Role.Explorer:
                _roleColorIndicator.color = Color.blue;
                break;
            case Role.Collector:
                _roleColorIndicator.color = Color.green;
                break;
            case Role.Tactical:
                _roleColorIndicator.color = Color.red;
                break;
            default:
                _roleColorIndicator.color = Color.clear; // Hide if the role doesn't have a specific color
                break;
        }
    }

    public void UpdateRoleDependentUI()
    {
        _batteryCountText.gameObject.SetActive(_player.currentRole == Role.Collector);
        _energyBar.gameObject.SetActive(_player.currentRole == Role.Explorer);
        if (_player.currentRole == Role.Tactical)
        {
            SetCrosshairVisibility(!_tacticalControl.IsTacticalModeActive);
            _exitTacticalButton.gameObject.SetActive(_tacticalControl.IsTacticalModeActive);
        }
    }

    public void SetCrosshairVisibility(bool isVisible)
    {
        if (_crosshairImage != null)
        {
            _crosshairImage.enabled = isVisible;
        }
    }

    private void UpdateEnergyUI()
    {
        if (_energyBar != null)
        {
            _energyBar.rectTransform.sizeDelta = new Vector2(110 * (_player.CurrentEnergy / _player.MaxEnergy), 20);
        }
    }

    private void UpdateGameTimeUI()
    {
        // Update the game time text based on the game time from the Player or GameManager
        _gameTimeText.text = _player.GetFormattedGameTime();
    }

    private void UpdateCoinsCollectedUI()
    {
        _coinsCollectedText.text = $"Coins: {_gameManager.CoinsCollected}";
    }

    public void UpdateBatteryRecharge()
    {
        _batteryCountText.text = $"Batteries: {_player.Batteries}";
    }

    public void UpdateRoleUI(Role newRole, TMP_Text roleText)
    {
        if (_roleUIText != null)
        {
            roleText.text = newRole.ToString();
            _roleUIText.text = newRole.ToString();
        }
        if (_roleColorIndicator != null)
        {
            UpdateRoleColorIndicator(newRole);
        }
    }

    public void SetEnergyBarVisibility(bool isVisible)
    {
        if (_energyBar != null)
        {
            _energyBar.enabled = isVisible;
        }
    }

    private void OnExitTacticalButtonClicked()
    {
        if (_tacticalControl != null)
        {
            _tacticalControl.IsTacticalModeActive = false;
        }
        _exitTacticalButton.gameObject.SetActive(false);
    }

    public void DisplayTrialOverScreen()
    {
       // Now create the "Trial Over" screen
        GameObject trialOverPanelObject = new GameObject("TrialOverPanel");
        Image trialOverPanel = trialOverPanelObject.AddComponent<Image>();
        trialOverPanel.color = Color.white; // Set the color to white

        RectTransform rt = trialOverPanel.GetComponent<RectTransform>();
        rt.SetParent(_mainCanvas.transform, false);
        rt.SetAsLastSibling(); // Ensure it's on top
        rt.sizeDelta = new Vector2(Screen.width, Screen.height); // Make it full screen

        // Make sure to add a CanvasRenderer
        // trialOverPanelObject.AddComponent<CanvasRenderer>();
        // TextMeshProUGUI trialOverText = trialOverPanelObject.AddComponent<TextMeshProUGUI>();

        // Add TextMeshProUGUI component to a child GameObject
        GameObject textObject = new GameObject("TrialOverText");
        textObject.transform.SetParent(trialOverPanelObject.transform, false);

        TextMeshProUGUI trialOverText = textObject.AddComponent<TextMeshProUGUI>();
        // Assign a TMP_FontAsset here, which you would usually get from your resources or settings
        // trialOverText.font = ...;
        trialOverText.text = "Trial Over";
        trialOverText.fontSize = 32;
        trialOverText.color = Color.black;
        trialOverText.alignment = TextAlignmentOptions.Center;
        trialOverText.enableAutoSizing = true; // Optional: Enable auto-sizing for the font
        trialOverText.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
    }

}
