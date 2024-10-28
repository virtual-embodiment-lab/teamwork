// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class AdversialAgent : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;
    [SerializeField] float time_before_start = 10;
    [SerializeField] float dist_to_chase = 5;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag("Player").transform;

        if (target == null)
        {
            Debug.LogError("Target is not assigned.");
        }

    }

    // Update is called once per frame
    void Update()
    {
        time_before_start -= Time.deltaTime;
        float dist = Vector3.Distance(target.position, agent.transform.position);
        if (time_before_start <= 0 || dist <= dist_to_chase)
        {
            agent.SetDestination(target.position);
        }   
     
    }
}
