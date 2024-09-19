// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System.Collections;
using Normal.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

/*
 * Coin instance manager
 */
public class CoinSample : RealtimeComponent<CoinModel>
{
    [SerializeField] int rotationSpeed = 20;

    void Update()
    {
        transform.Rotate(new Vector3(rotationSpeed, rotationSpeed, rotationSpeed) * Time.deltaTime);
    }

}
