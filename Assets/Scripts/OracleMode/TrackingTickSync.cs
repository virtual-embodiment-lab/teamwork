using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class TrackingTickSync : RealtimeComponent<TrackingTickModel>
{
    public int _trackingTick;

    private void Awake()
    {
        _trackingTick = 0;
    }

    protected override void OnRealtimeModelReplaced(TrackingTickModel previousModel, TrackingTickModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.trackingTickDidChange -= TrackingTickDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
                currentModel.trackingTick = _trackingTick;
            }

            UpdateTrackingTick();
            currentModel.trackingTickDidChange += TrackingTickDidChange;
        }
    }

    private void TrackingTickDidChange(TrackingTickModel model, int value)
    {
        UpdateTrackingTick();
    }

    private void UpdateTrackingTick()
    {
        _trackingTick = model.trackingTick;
    }

    public void SetTrackingTick(int value)
    {
        model.trackingTick = value;
    }

    public int GetTrackingTick()
    {
        return model.trackingTick;
    }

}