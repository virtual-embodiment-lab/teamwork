using UnityEngine;
using Normal.Realtime;

public class CustomAvatarSpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public RealtimeAvatarManager avatarManager;
    public OVRPlayerController ovrPlayerController;

    void Start()
    {
        // Find the RealtimeAvatarManager in the scene
        avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        if (avatarManager == null)
        {
            Debug.LogError("CustomAvatarSpawnManager: No RealtimeAvatarManager found in the scene.");
            return;
        }

        // Subscribe to the avatarCreated event
        avatarManager.avatarCreated += OnAvatarCreated;

        // Find the OVRPlayerController in the scene
        ovrPlayerController = FindObjectOfType<OVRPlayerController>();
        if (ovrPlayerController == null)
        {
            Debug.LogError("CustomAvatarSpawnManager: No OVRPlayerController found in the scene.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the avatarCreated event when this object is destroyed
        if (avatarManager != null)
        {
            avatarManager.avatarCreated -= OnAvatarCreated;
        }
    }

    private void OnAvatarCreated(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        if (isLocalAvatar && ovrPlayerController != null)
        {
            // Choose a random spawn point
            if (spawnPoints.Length > 0)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                // Set the avatar's position and rotation
                avatar.transform.position = spawnPoint.position;
                avatar.transform.rotation = spawnPoint.rotation;

                // Synchronize the OVRPlayerController's position and rotation with the avatar
                ovrPlayerController.transform.position = spawnPoint.position;
                ovrPlayerController.transform.rotation = spawnPoint.rotation;
            }
            else
            {
                Debug.LogWarning("CustomAvatarSpawnManager: No spawn points set.");
            }
        }
    }
}
