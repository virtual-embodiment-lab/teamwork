using UnityEngine;

public class RoleTrigger : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
    //private void OnTriggerEnter()
    {
        Debug.Log("role");
        
        if (other.CompareTag("Player"))
        {
            RoleSelect startRoom = FindObjectOfType<RoleSelect>(); // Find the StartRoom script in the scene.
            Debug.Log(startRoom);
            if (startRoom != null)
            {
                startRoom.HandlePlayerEnterTrigger(GetComponent<Collider>(), other.gameObject);
            }
        }
        
    }
}
