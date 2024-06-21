using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collideOverLay : MonoBehaviour
{

  private void OnTriggerEnter(Collider other)
    //private void OnTriggerEnter()
    {
        Debug.Log(other);
        /*
        if (other.CompareTag("Player"))
        {
            RoleSelect startRoom = FindObjectOfType<RoleSelect>(); // Find the StartRoom script in the scene.
            Debug.Log(startRoom);
            if (startRoom != null)
            {
                startRoom.HandlePlayerEnterTrigger(GetComponent<Collider>(), other.gameObject);
            }
        }
        */
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
