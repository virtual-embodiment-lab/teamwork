// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

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
