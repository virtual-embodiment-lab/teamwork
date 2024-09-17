using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        // Find the active camera each frame, in case it changes.
        Camera mainCamera = Camera.current;

        // If no active camera is found, try the main camera.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // If a camera is found, rotate the text object to face the camera.
        if (mainCamera != null)
        {
            Vector3 targetPosition = new Vector3(mainCamera.transform.position.x,
                                                 transform.position.y,
                                                 mainCamera.transform.position.z);

            // Rotate the text to face the target position, then flip it 180 degrees to face the camera.
            transform.LookAt(targetPosition, Vector3.up);
            transform.Rotate(0, 180f, 0); // This will flip the text to face the camera correctly.
        }
    }
}
