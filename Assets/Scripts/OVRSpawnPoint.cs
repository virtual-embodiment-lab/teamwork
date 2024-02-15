using System.Linq;
using UnityEngine;
using Normal.Realtime;

public class OVRSpawnPoint : MonoBehaviour
{
    public Realtime _realtime;
    public SpawnPoint[] spawnPoints;
    public RealtimeAvatarManager avatarManager;
    private bool _hasInitializedSpawn = false;
    private CharacterController _characterController;

    private void Start()
    {
        // Retrieve all objects with the SpawnPoint script
        spawnPoints = FindObjectsOfType<SpawnPoint>();
        // Subscribe to the avatarCreated event from the avatar manager
        avatarManager.avatarCreated += OnAvatarCreated;
        // Try to get the CharacterController component
        _characterController = GetComponent<CharacterController>();
    }

    private void OnAvatarCreated(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        // Check if the created avatar is the local avatar and the spawn hasn't been initialized
        if (isLocalAvatar && !_hasInitializedSpawn)
        {
            _hasInitializedSpawn = true; // Prevent multiple initializations
            if (_characterController != null)
            {
                _characterController.enabled = false; // Temporarily disable the character controller
            }
            MovePlayerToSpawnPoint(gameObject); // Move the avatar to the spawn point
        }
    }

    private void MovePlayerToSpawnPoint(GameObject avatarGameObject)
    {
        // Filter out the overflow points and find the first unoccupied spawn point
        SpawnPoint spawnPoint = spawnPoints
            .Where(sp => !sp.IsOverflow()) // Ensure it's not an overflow point
            .FirstOrDefault(sp => !sp.IsOccupied()); // Find the first unoccupied point

        // If all regular spawn points are occupied, use the overflow spawn point
        if (spawnPoint == null)
        {
            spawnPoint = spawnPoints.FirstOrDefault(sp => sp.IsOverflow());
        }

        // If a spawn point is found, move the player and set the occupation status
        if (spawnPoint != null)
        {
            avatarGameObject.transform.position = spawnPoint.transform.position;
            avatarGameObject.transform.rotation = spawnPoint.transform.rotation;
            spawnPoint.SetOccupationStatus(true, _realtime.clientID);
            if (_characterController != null)
            {
                _characterController.enabled = true;
            }
        }
        else
        {
            Debug.LogError("No spawn points available!");
        }
    }

    // When the player leaves or disconnects
    private void OnDisable()
    {
        // Retrieve all objects with the SpawnPoint script
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        // Find the spawn point that the player is occupying
        SpawnPoint occupiedSpawnPoint = spawnPoints
            .FirstOrDefault(sp => sp.GetOccupyingClientID() == _realtime.clientID);

        // Reset the occupation status if the player is occupying a spawn point
        if (occupiedSpawnPoint != null)
        {
            occupiedSpawnPoint.SetOccupationStatus(false, -1);
        }
    }
}
