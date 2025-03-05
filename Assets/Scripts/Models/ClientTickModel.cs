using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RealtimeModel]
public partial class ClientTickModel
{
    [RealtimeProperty(1, true, true)]
    private int _currentTick;
}