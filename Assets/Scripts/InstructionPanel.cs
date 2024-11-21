// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;

public class InstructionPanel : MonoBehaviour
{
    //[SerializeField] private Material mat;
    [SerializeField] OVRPlayerController avatarController;
    [SerializeField] public List<Texture> instructionForAll = new List<Texture>();
    [SerializeField] public List<Texture> instructionForExplorer = new List<Texture>();
    [SerializeField] public List<Texture> instructionForCollector = new List<Texture>();
    [SerializeField] public List<Texture> instructionForTactical = new List<Texture>();
    private Role roleInstruction = Role.None;

    private bool showInstruction = true;
    private Renderer objRenderer;

    // Start is called before the first frame update
    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        objRenderer.enabled = showInstruction;

        if (instructionForAll.Count > 0)
        {
            objRenderer.material.mainTexture = instructionForAll[currentSlide];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetUp(OVRInput.RawButton.B))
        {
            showInstruction = !showInstruction;
            objRenderer.enabled = showInstruction;
            if (showInstruction)
            {
                Transform player = GameObject.Find("OVRPlayerController").transform;
                Vector3 plPosition = player.position;
                transform.position = plPosition + new Vector3(0f, 1.5f, 0f) + player.forward * 1.5f;
                transform.rotation = Quaternion.LookRotation(player.forward, Vector3.up);
            }  
        }
        else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
        {
            GetComponent<Renderer>().material.mainTexture = instructionForAll[1];
            Debug.Log("test");

            currentSlide++;
            objRenderer.material.mainTexture = instructionForAll[currentSlide];
        }
        else if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
        {
            
        }
    }

    public void ChangeRole(Role role)
    {
        roleInstruction = role;
    }

    public void createInstructionPanel()
    {
        Transform player = GameObject.Find("OVRPlayerController").transform;
        Vector3 plPosition = player.position;
        transform.position = plPosition + new Vector3(0f, 1.5f, 0f) + player.forward * 1.5f;
        transform.rotation = Quaternion.LookRotation(player.forward, Vector3.up);
    }

    public void DisplayInstruction()
    {
        if (currentSlide >= 6 && currentSlide <= 9)
        {
            transform.position = new Vector3(4.224f, 1.329f, 0.003f);
            transform.rotation = Quaternion.Euler(-0.013f, -88.058f, 1.001f);
        }

        //explorer and collector
        if (roleInstruction == Role.explorer && (currentSlide >= 10 && currentSlide <= 12 || currentSlide >= 17 && currentSlide <= 18))
        {
            transform.position = new Vector3(0.013f, 1.217f, -0.627f);
            transform.rotation = Quaternion.Euler(0f, -180.94f, 0f);
        }

        if (roleInstruction == Role.collector && (currentSlide == 10 || (currentSlide >= 13 && currentSlide <= 16) || currentSlide == 19))
        {
            transform.position = new Vector3(0.013f, 1.217f, -0.627f);
            transform.rotation = Quaternion.Euler(0f, -180.94f, 0f);
        }

        //tactical
        if (roleInstruction == Role.tactical && (currentSlide == 20 && currentSlide == 21)) {
            transform.position = new Vector3(0.013f, 1.217f, -0.627f);
            transform.rotation = Quaternion.Euler(0f, -180.94f, 0f);
        }

        if (roleInstruction == Role.tactical && currentSlide >= 22 && currentSlide <= 26) {
            transform.position = new Vector3(-0.04f, 1.29f, -5.41f);
            transform.rotation = Quaternion.Euler(0f, -180.94f, 0f);
        }

        if (currentSlide >= 27)
        {
            transform.position = new Vector3(-0.843f, 1.217f, 0.028f);
            transform.rotation = Quaternion.Euler(0f, -88.341f, 0f);
        }
    }
}
