// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using UnityEngine;

/*
 * Destroy the particles after instantiation
 */
public class ParticleTimer : MonoBehaviour
{
    [Header("Delay Time")]
    [SerializeField] public float delay = 2.0f;
    private void Start()
    {
        Destroy(gameObject, delay);
    }
}