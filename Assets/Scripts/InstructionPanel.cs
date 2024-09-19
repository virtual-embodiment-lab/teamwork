// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InstructionPanel : MonoBehaviour
{
    //[SerializeField] private Material mat;

    [SerializeField] public List<Texture> instructionForAll = new List<Texture>();
    [SerializeField] public List<Texture> instructionForExplorer = new List<Texture>();
    [SerializeField] public List<Texture> instructionForCollector = new List<Texture>();
    [SerializeField] public List<Texture> instructionForTactical = new List<Texture>();
    private Role roleInstruction = Role.None;
    private bool showInstruction = true;
    public InputActionReference BButtonAction;
    private Renderer objRenderer;

     void Awake()
    {
        BButtonAction.action.performed += OnBButtonPressed;
        BButtonAction.action.Enable();
    }

    void onDestroy()
    {
        BButtonAction.action.performed -= OnBButtonPressed;
        BButtonAction.action.Disable();
    }

    private void OnBButtonPressed(InputAction.CallbackContext context)
    {
        showInstruction = !showInstruction;
        objRenderer.enabled = showInstruction;
        if (showInstruction)
        {
            Transform player = GameObject.Find("XR Origin (XR Rig) teleport").transform;
            Vector3 plPosition = player.position;
            transform.position = plPosition + new Vector3(0f, 1.5f, 0f) + player.forward * 1.5f;
            transform.rotation = Quaternion.LookRotation(player.forward, Vector3.up);
        }        
    }
    // Start is called before the first frame update
    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        objRenderer.enabled = showInstruction;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeRole(Role role)
    {
        roleInstruction = role;
    }
    public void createInstructionPanel()
    {
        Transform player = GameObject.Find("XR Origin (XR Rig) teleport").transform;
        Vector3 plPosition = player.position;
        transform.position = plPosition + new Vector3(0f, 1.5f, 0f) + player.forward * 1.5f;
        transform.rotation = Quaternion.LookRotation(player.forward, Vector3.up);
    }
}
