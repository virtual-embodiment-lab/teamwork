// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using Normal.Realtime;
using UnityEngine;

/*
 * Coin instance manager
 */
public class BatterySample : MonoBehaviour
{
    [SerializeField] int rotationSpeed = 20;

    void Update()
    {
        transform.Rotate(new Vector3(rotationSpeed, rotationSpeed, rotationSpeed) * Time.deltaTime);
    }
}
