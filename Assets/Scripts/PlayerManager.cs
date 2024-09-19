// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

#if NORMCORE

using UnityEngine;
using Normal.Realtime;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _spawnLocation;

    private Realtime _realtime;

    private void Awake()
    {
        // Get the Realtime component on this game object
        _realtime = GetComponent<Realtime>();

        // Notify us when Realtime successfully connects to the room
        _realtime.didConnectToRoom += DidConnectToRoom;
    }

    private void DidConnectToRoom(Realtime realtime)
    {
        // Instantiate the CubePlayer for this client once we've successfully connected to the room. Position it 1 meter in the air.
        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,    // Make sure the RealtimeView on this prefab is owned by this client.
            preventOwnershipTakeover = true,    // Prevent other clients from calling RequestOwnership() on the root RealtimeView.
            useInstance = realtime // Use the instance of Realtime that fired the didConnectToRoom event.
        };
        Realtime.Instantiate(_prefab.name, _spawnLocation.position, Quaternion.identity, options);
    }
}

#endif