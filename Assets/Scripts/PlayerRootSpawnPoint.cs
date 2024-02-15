using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRootSpawnPoint : MonoBehaviour
{
    public List<SpawnPoint> spawnPoints;
    public Transform overflowSpawnPoint; // Changed to Transform type
    private int selectedSpawnIndex = 0; // Index of the selected spawn point

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f); // Wait for VR initialization
        MovePlayerToSpawnPoint(); // Your method to move the player
    }
    //private void Awake()
    //{
    //}

    void MovePlayerToSpawnPoint()
    {
        // Fetch spawn points based on tag
        spawnPoints = new List<SpawnPoint>(GameObject.FindGameObjectsWithTag("SpawnPoint").TransformToSpawnPoint());

        // Fetch the overflow spawn point as a transform
        GameObject overflowObject = GameObject.FindGameObjectWithTag("OverflowSpawnPoint");
        if (overflowObject != null)
        {
            overflowSpawnPoint = overflowObject.transform;
        }

        // Find OVRPlayerController in the scene
        GameObject ovrPlayerController = GameObject.FindObjectOfType<OVRPlayerController>().gameObject;

        if (ovrPlayerController != null)
        {
            // Move OVRPlayerController to the position and rotation of the selected spawn point
            if (spawnPoints.Count > selectedSpawnIndex)
            {
                Transform selectedSpawn = spawnPoints[selectedSpawnIndex].transform;
                ovrPlayerController.transform.position = selectedSpawn.position;
                ovrPlayerController.transform.rotation = selectedSpawn.rotation;
            }
            else if (overflowSpawnPoint != null) // Fallback to overflow spawn point if no valid spawn points are found
            {
                ovrPlayerController.transform.position = overflowSpawnPoint.position;
                ovrPlayerController.transform.rotation = overflowSpawnPoint.rotation;
            }
        }
        else
        {
            Debug.LogError("OVRPlayerController not found in the scene!");
        }
    }
}

public static class ExtensionMethods
{
    // Extension method to convert an array of GameObjects to a list of SpawnPoints
    public static List<SpawnPoint> TransformToSpawnPoint(this GameObject[] gameObjects)
    {
        List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
        foreach (var gameObject in gameObjects)
        {
            SpawnPoint sp = gameObject.GetComponent<SpawnPoint>();
            if (sp != null)
            {
                spawnPoints.Add(sp);
            }
        }
        return spawnPoints;
    }
}
