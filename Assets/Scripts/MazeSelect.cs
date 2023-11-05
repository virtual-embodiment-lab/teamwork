using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using Normal.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;

public class MazeSelect : MonoBehaviour
{
    [SerializeField] public GameObject largeMaze;
    [SerializeField] public GameObject mediumMaze;
    [SerializeField] public GameObject smallMaze;
    [SerializeField] public TMP_Text largeMazeButton;
    [SerializeField] public TMP_Text mediumMazeButton;
    [SerializeField] public TMP_Text smallMazeButton;

    [SerializeField] private Camera uiCamera; // This will be set when the player is found
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;

    void Start()
    {
        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        // Start the coroutine to find the locally owned player's camera
        StartCoroutine(FindLocalPlayerCamera());
    }

    void Update()
    {
        if (uiCamera == null) return; // If the camera hasn't been found yet, don't proceed

        if (IsCrosshairOverUI(out TMP_Text hitButton))
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (hitButton == largeMazeButton)
                {
                    ActivateMaze(largeMaze);
                }
                else if (hitButton == mediumMazeButton)
                {
                    ActivateMaze(mediumMaze);
                }
                else if (hitButton == smallMazeButton)
                {
                    ActivateMaze(smallMaze);
                }
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
                    // Assuming the camera is a direct child of the player object
                    uiCamera = view.GetComponentInChildren<Camera>();
                    break; // Stop the loop once the local player camera is found
                }
            }

            if (uiCamera == null)
            {
                // Wait for a short time before trying again if no camera is found
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private bool IsCrosshairOverUI(out TMP_Text hitButton)
    {
        hitButton = null;
        if (uiCamera == null) return false;

        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = new Vector2(uiCamera.pixelWidth / 2, uiCamera.pixelHeight / 2)
        };

        var results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            hitButton = result.gameObject.GetComponent<TMP_Text>();
            if (hitButton != null)
                return true;
        }

        return false;
    }

    private void ActivateMaze(GameObject maze)
    {
        // Deactivate all mazes
        largeMaze.SetActive(false);
        mediumMaze.SetActive(false);
        smallMaze.SetActive(false);

        // Activate the selected maze
        maze.SetActive(true);
    }
}
