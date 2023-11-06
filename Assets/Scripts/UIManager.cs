using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager: MonoBehaviour
{
    private Player _player;
    private Camera _playerCamera;
    private TMP_Text _roleText;
    private Sprite _crosshairSprite;

    private Image _roleColorIndicator;
    private Image _uiPanelBackground;
    private Image _crosshairImage;
    private Image _energyBar;
    private TextMeshProUGUI _gameTimeText;
    private TextMeshProUGUI _roleUIText;
    private TextMeshProUGUI _coinsCollectedText;
    private TextMeshProUGUI _batteryCountText;
    private RectTransform _uiPanel;
    private Canvas _mainCanvas;

    public void Initialize(Player player, Camera playerCamera, TMP_Text roleText, Sprite crosshairSprite)
    {
        _player = player;
        _playerCamera = playerCamera;
        _roleText = roleText;
        _crosshairSprite = crosshairSprite;

        InitializeMainCanvas();
        CreateCrosshairUI();
        CreateMainUIPanel();
        CreateGameTimeUI();
        CreateRoleUI();
        CreateRoleColorIndicator();
        CreateCoinsCollectedUI();
        CreateBatteryCountUI();
        CreateEnergyBarUI();
        UpdateEnergyUI();
        UpdateRoleDependentUI();
    }

    public void UpdateUI()
    {
        UpdateGameTimeUI();
        UpdateCoinsCollectedUI();
        UpdateBatteryRecharge();
        UpdateEnergyUI();
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
        _uiPanel.sizeDelta = new Vector2(100, 60); // Adjust the width to be less wide
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
        _gameTimeText = CreateUIElement<TextMeshProUGUI>("GameTimeText", new Vector2(0, 1), new Vector2(0.5f, 1), new Vector2(10, -10), new Vector2(-10, -10));
        _gameTimeText.text = "00:00";
    }

    private void CreateRoleUI()
    {
        _roleUIText = CreateUIElement<TextMeshProUGUI>("RoleText", new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -50), new Vector2(100, -10)); // Adjusted for a smaller width
        _roleUIText.alignment = TextAlignmentOptions.Left;
        _roleUIText.text = _player.currentRole.ToString();
    }


    private void CreateCoinsCollectedUI()
    {
        _coinsCollectedText = CreateUIElement<TextMeshProUGUI>("CoinsCollectedText", new Vector2(0.5f, 1), new Vector2(1, 1), new Vector2(10, -10), new Vector2(-10, -10));
        _coinsCollectedText.text = "Coins: 0";
    }

    private void CreateBatteryCountUI()
    {
        _batteryCountText = CreateUIElement<TextMeshProUGUI>("BatteryCountText", new Vector2(0.5f, 1), new Vector2(1, 1), new Vector2(10, -60), new Vector2(-10, -10));
        _batteryCountText.text = $"Batteries: {_player.Batteries}/{Player.MaxBatteries}";
    }

    private void CreateEnergyBarUI()
    {
        GameObject energyBarObject = new GameObject("EnergyBar");
        _energyBar = energyBarObject.AddComponent<Image>();
        _energyBar.color = Color.green;
        SetupUIElement(_energyBar.rectTransform, new Vector2(0, 1), new Vector2(0.5f, 1), new Vector2(10, -110), new Vector2(-10, -10));
        _energyBar.enabled = false;
        energyBarObject.transform.SetParent(_uiPanel, false);
    }

    private void CreateRoleColorIndicator()
    {
        GameObject colorIndicatorObject = new GameObject("RoleColorIndicator");
        _roleColorIndicator = colorIndicatorObject.AddComponent<Image>();
        RectTransform rt = _roleColorIndicator.GetComponent<RectTransform>();

        // Assuming _roleUIText has already been created and has a defined height
        float textHeight = _roleUIText.preferredHeight;

        // Set the rectangle to the right of the RoleText and as tall as the RoleText
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 0.5f);
        rt.sizeDelta = new Vector2(20, textHeight); // Width of 20 and height as tall as the RoleText
        rt.anchoredPosition = new Vector2(_roleUIText.rectTransform.anchoredPosition.x + _roleUIText.rectTransform.sizeDelta.x + 10, _roleUIText.rectTransform.anchoredPosition.y);

        colorIndicatorObject.transform.SetParent(_uiPanel.transform, false);
        UpdateRoleColorIndicator(_player.currentRole); // Set initial color
    }


    private T CreateUIElement<T>(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax) where T : Component
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
        rt.localScale = Vector3.one;

        uiElementObject.transform.SetParent(_uiPanel, false);

        // Specific setup for TextMeshProUGUI
        if (typeof(T) == typeof(TextMeshProUGUI))
        {
            var textElement = uiElement as TextMeshProUGUI;
            textElement.fontSize = 18;
            textElement.color = Color.white;
            textElement.alignment = TextAlignmentOptions.Center;
        }

        return uiElement;
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

    private void UpdateRoleDependentUI()
    {
        bool isCollector = _player.currentRole == Role.Collector;
        _batteryCountText.gameObject.SetActive(isCollector); // Only active if the player is a collector

        // If there is an explorer energy bar or other role-specific UI elements, update their visibility here as well
        _energyBar.gameObject.SetActive(_player.currentRole == Role.Explorer);
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
            _energyBar.rectTransform.sizeDelta = new Vector2(200 * (_player.CurrentEnergy / _player.MaxEnergy), 20);
        }
    }

    private void UpdateGameTimeUI()
    {
        // Update the game time text based on the game time from the Player or GameManager
        _gameTimeText.text = _player.GetFormattedGameTime();
    }

    private void UpdateCoinsCollectedUI()
    {
        _coinsCollectedText.text = $"Coins: 0";
    }

    private void UpdateBatteryRecharge()
    {
        _batteryCountText.text = $"Batteries: {_player.Batteries}/{Player.MaxBatteries}";
    }

    public void UpdateRoleUI(Role newRole)
    {
        if (_roleUIText != null)
        {
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

}
