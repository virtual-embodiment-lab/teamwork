using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contorllerSwitcher : MonoBehaviour
{
    public OVRPlayerController avatarController;
    public GameObject cameraController;
    private bool controllingAvatar = true;
    private CameraController tacticalController;

    private void OnTriggerEnter(Collider other)
    //private void OnTriggerEnter()
    {
        Debug.Log(other);
        
        {
            controllingAvatar = !controllingAvatar;
            avatarController.EnableLinearMovement = controllingAvatar;
            avatarController.EnableRotation = controllingAvatar;
            //avatarController.SetActive(controllingAvatar);
            tacticalController.enableControl = !controllingAvatar;
            Debug.Log("switch");
        }
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        tacticalController = cameraController.GetComponent<CameraController>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
