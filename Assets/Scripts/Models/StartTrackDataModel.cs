using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RealtimeModel]
public partial class StartTrackDataModel
{
    [RealtimeProperty(1, true, true)]
    private bool _isTracking;
}