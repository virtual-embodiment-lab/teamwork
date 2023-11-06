using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using Normal.Realtime;
using System.Collections.Generic;
using UnityEngine.UI;

public class MazeSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] public TMP_Text largeMazeButton;
    [SerializeField] public TMP_Text mediumMazeButton;
    [SerializeField] public TMP_Text smallMazeButton;
    [SerializeField] private MazeStateSync mazeStateSync;

    private Camera uiCamera;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;
    private TMP_Text currentHoveredButton;
    private bool isGameStarted = false;
    private StartTrigger startTrigger;

    void Start()
    {
        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
        StartCoroutine(FindLocalPlayerCamera());
        startTrigger = FindObjectOfType<StartTrigger>();
        if (startTrigger != null)
        {
            startTrigger.OnGameStarted += HandleGameStarted;
        }
    }

    private void OnDestroy()
    {
        if (startTrigger != null)
        {
            startTrigger.OnGameStarted -= HandleGameStarted;
        }
    }

    private void HandleGameStarted()
    {
        isGameStarted = true;
    }

    void Update()
    {
        if (isGameStarted) return;
        // Check if key "1" is pressed
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateMaze(0); // Assume this corresponds to the small maze
        }
        // Check if key "2" is pressed
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateMaze(1); // Assume this corresponds to the medium maze
        }
        // Check if key "3" is pressed
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActivateMaze(2); // Assume this corresponds to the large maze
        }

        if (uiCamera == null) return;

        // Detect if the crosshair is over a UI button
        if (IsCrosshairOverUI())
        {
            if (Input.GetButtonDown("Fire1") && currentHoveredButton != null)
            {
                OnPointerClick(new PointerEventData(eventSystem));
            }
        }
    }

    private IEnumerator FindLocalPlayerCamera()
    {
        while (uiCamera == null)
        {
            RealtimeView[] playerViews = FindObjectsOfType<RealtimeView>();
            foreach (RealtimeView view in playerViews)
            {
                if (view.isOwnedLocallyInHierarchy)
                {
                    uiCamera = view.GetComponentInChildren<Camera>();
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool IsCrosshairOverUI()
    {
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = new Vector2(uiCamera.pixelWidth / 2, uiCamera.pixelHeight / 2)
        };

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            TMP_Text hitButton = result.gameObject.GetComponent<TMP_Text>();
            if (hitButton != null)
            {
                if (currentHoveredButton != hitButton)
                {
                    if (currentHoveredButton != null)
                    {
                        OnPointerExit(new PointerEventData(eventSystem)); // Simulate pointer exit for the previous button
                    }
                    currentHoveredButton = hitButton;
                    OnPointerEnter(new PointerEventData(eventSystem)); // Simulate pointer enter for the new button
                }
                return true;
            }
        }

        if (currentHoveredButton != null)
        {
            OnPointerExit(new PointerEventData(eventSystem)); // Simulate pointer exit if no button is hovered
            currentHoveredButton = null;
        }

        return false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //// Trigger the highlight state of the button
        //TMP_Text button = currentHoveredButton.GetComponent<TMP_Text>();
        //if (button != null)
        //{
        //    // Your code to change the color or visual state when the button is highlighted
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //// Trigger the normal state of the button
        //TMP_Text button = currentHoveredButton.GetComponent<TMP_Text>();
        //if (button != null)
        //{
        //    // Your code to revert the color or visual state when the button is no longer highlighted
        //}
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Trigger the pressed state of the button and activate the corresponding maze
        TMP_Text button = currentHoveredButton.GetComponent<TMP_Text>();
        if (button != null)
        {
            // Your code to change the color or visual state when the button is pressed
            if (button == largeMazeButton)
            {
                ActivateMaze(2);
            }
            else if (button == mediumMazeButton)
            {
                ActivateMaze(1);
            }
            else if (button == smallMazeButton)
            {
                ActivateMaze(0);
            }
        }
    }

    private void ActivateMaze(int index)
    {
        mazeStateSync.SetActiveMaze(index);
    }
}
