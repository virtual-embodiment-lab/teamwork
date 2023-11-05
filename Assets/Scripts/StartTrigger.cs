using UnityEngine;

public class StartTrigger : MonoBehaviour
{
    [SerializeField] private MazeStateSync mazeStateSync; // Reference to MazeSelect script
    [SerializeField] private RoleSelect roleSelect; // Reference to RoleSelect script
    [SerializeField] private GameObject door;       // The door GameObject to deactivate

    private bool started = false;
    private int totalPlayers = 0;
    private int playersInTrigger = 0;

    void Awake()
    {
        // Count all players in the game at start
        totalPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInTrigger++;
            CheckStartConditions();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInTrigger--;
        }
    }

    void CheckStartConditions()
    {
        // Check if the game is not started and all conditions are met to start the game
        if (!started && IsEveryoneReady())
        {
            started = true;
            StartGame();
        }
    }

    private bool IsEveryoneReady()
    {
        // Check if a maze has been selected
        if (!mazeStateSync.IsMazeSelected())
            return false;

        // Check if all players have roles
        if (!roleSelect.AreAllRolesAssigned())
            return false;

        // Check if all players are within the trigger
        if (playersInTrigger < totalPlayers)
            return false;

        // All conditions are met
        return true;
    }

    private void StartGame()
    {
        // Deactivate the door
        if (door != null)
            door.SetActive(false);

        // Any other start game logic can go here
    }
}
