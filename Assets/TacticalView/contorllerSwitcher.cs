// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class contorllerSwitcher : MonoBehaviour
{
    public OVRPlayerController avatarController;
    public GameObject cameraController;
    public bool controllingAvatar = true;
    private CameraController tacticalController;
    private Player _player;

    private void OnTriggerEnter(Collider other)
    //private void OnTriggerEnter()
    {
        // Get the RealtimeView component from the collider
        RealtimeView realtimeView = other.GetComponent<RealtimeView>();
        if (realtimeView != null)
        {
            // Check if the RealtimeView is owned by the local player
            if (realtimeView.isOwnedLocallySelf)
            {
                _player = other.GetComponent<Player>();
                if (_player.currentRole == Role.Tactical){
                   switchMode(true);
                }
            }
        }        
    }
    
    public void switchMode(bool controllerMode)
    {
        controllingAvatar = !controllingAvatar;
        Logger_new lg = _player.GetComponent<Logger_new>();
        lg.AddLine("tacticalMode:" + !controllingAvatar);

        avatarController.EnableLinearMovement = controllingAvatar;
        avatarController.EnableRotation = controllingAvatar;
        tacticalController.enableControl = !controllingAvatar;
        Debug.Log("switch");
    }

    // Start is called before the first frame update
    void Start()
    {
        tacticalController = cameraController.GetComponent<CameraController>();
    }
}
