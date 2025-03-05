using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class StartTrackingSync : RealtimeComponent<StartTrackDataModel>
{
    public bool _isTracking;

    private void Awake()
    {
        _isTracking = false;
    }

    protected override void OnRealtimeModelReplaced(StartTrackDataModel previousModel, StartTrackDataModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.isTrackingDidChange -= IsTrackingDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
                currentModel.isTracking = _isTracking;
            }

            UpdateTrackingStatus();
            currentModel.isTrackingDidChange += IsTrackingDidChange;
        }
    }

    private void IsTrackingDidChange(StartTrackDataModel model, bool value)
    {
        UpdateTrackingStatus();
    }

    private void UpdateTrackingStatus()
    {
        _isTracking = model.isTracking;
    }

    public void SetTracking(bool value)
    {
        model.isTracking = value;
    }

    public bool GetTracking()
    {
        return model.isTracking;
    }
}