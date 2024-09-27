// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using Normal.Realtime;
using UnityEngine;

public class SpawnPoint : RealtimeComponent<SpawnPointModel>
{
    [SerializeField] private bool overflow = false;
    [SerializeField] private bool occupied = false;
    [SerializeField] private int client = -1;

    private void Start()
    {
        if (model.isFreshModel)
        {
            model.isOccupied = false;
            model.occupyingClientID = -1;
        }
    }

    public void SetOccupationStatus(bool status, int clientID)
    {
        // Set the model properties; Normcore syncs these automatically
        model.isOccupied = status;
        model.occupyingClientID = status ? clientID : -1;

        // Update local properties for local reference (optional)
        occupied = status;
        client = clientID;
    }

    protected override void OnRealtimeModelReplaced(SpawnPointModel previousModel, SpawnPointModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events on the previous model
            previousModel.occupyingClientIDDidChange -= OccupyingClientIDDidChange;
        }

        if (currentModel != null)
        {
            // Update local state to match the new model
            occupied = currentModel.isOccupied;
            client = currentModel.occupyingClientID;

            // Register for events so we'll know if the model changes later
            currentModel.occupyingClientIDDidChange += OccupyingClientIDDidChange;
        }
    }

    private void OccupyingClientIDDidChange(SpawnPointModel model, int clientID)
    {
        // Update local state when the model changes
        occupied = model.isOccupied;
        client = clientID;
    }



    public int GetOccupyingClientID()
    {
        return model.occupyingClientID; 
    }

    public bool IsOverflow()
    {
        return overflow;
    }

    public bool IsOccupied()
    {
        return model.isOccupied;
    }

}
