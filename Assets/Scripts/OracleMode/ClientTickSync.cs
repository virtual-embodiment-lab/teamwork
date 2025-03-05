using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class ClientTickSync : RealtimeComponent<ClientTickModel>
{
    public int _currentTick = 0;

    protected override void OnRealtimeModelReplaced(ClientTickModel previousModel, ClientTickModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.currentTickDidChange -= CurrentTickDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
                currentModel.currentTick = _currentTick;
            }

            UpdateCurrentTick();
            currentModel.currentTickDidChange += CurrentTickDidChange;
        }
    }

    private void CurrentTickDidChange(ClientTickModel model, int value)
    {
        UpdateCurrentTick();
    }

    private void UpdateCurrentTick()
    {
        _currentTick = model.currentTick;
    }

    public void SetCurrentTick(int value)
    {
        model.currentTick = value;
    }

    public int GetCurrentTick()
    {
        return _currentTick;
    }
}