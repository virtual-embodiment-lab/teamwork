using Normal.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class AvatarTriggerDetector : MonoBehaviour
{
    // Stores all player GameObjects currently inside the trigger
    private HashSet<GameObject> playersInside = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // Check if the GameObject entering has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Add the GameObject to the set
            playersInside.Add(other.gameObject);
            CheckAllPlayersInside();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the GameObject exiting has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Remove the GameObject from the set
            playersInside.Remove(other.gameObject);
        }
    }

    private void CheckAllPlayersInside()
    {
        // Get all GameObjects with the "Player" tag in the scene
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        // Check if the number of players inside the trigger equals the total number of players
        if (playersInside.Count == allPlayers.Length)
        {
            Debug.Log("All players are inside the trigger area!");
            // Perform your logic here when all players are inside
        }
    }
}
