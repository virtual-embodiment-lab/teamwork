using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class BatteryBox : MonoBehaviour
{
    private Player _player;

    private void OnTriggerEnter(Collider other)
    {
        // Get the RealtimeView component from the collider
        RealtimeView realtimeView = other.GetComponent<RealtimeView>();
        if (realtimeView != null)
        {
            // Check if the RealtimeView is owned by the local player
            if (realtimeView.isOwnedLocallySelf)
            {
                _player = other.GetComponent<Player>();
                int batteries = _player.carryingBatteries;
            }
        }        
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
