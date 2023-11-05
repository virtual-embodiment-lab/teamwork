using UnityEngine;

public class RoleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RoleSelect startRoom = FindObjectOfType<RoleSelect>(); // Find the StartRoom script in the scene.
            if (startRoom != null)
            {
                startRoom.HandlePlayerEnterTrigger(GetComponent<Collider>(), other.gameObject);
            }
        }
    }
}
