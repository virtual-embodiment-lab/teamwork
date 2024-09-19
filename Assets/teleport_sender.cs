// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport_sender : MonoBehaviour
{
    public Transform player, destination;
    public GameObject playerg;

    // Start is called before the first frame update
    void onTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerg.SetActive(false);
            player.position = destination.position;
            playerg.SetActive(true);
        }
    }
}
