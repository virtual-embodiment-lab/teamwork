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
    private int currentSlide  = 0;    // Current slide number

    private List<Texture> GetRoleInstructionList()
    {
        switch (roleInstruction)
        {
            case Role.Explorer:
                return instructionForExplorer;
            case Role.Collector:
                return instructionForCollector;
            case Role.Tactical:
                return instructionForTactical;
            default:
                return instructionForAll;  // Default if no specific role is set
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            objRenderer.enabled = showInstruction;
            // Initialize panel with first image from 'instructionForAll'
            if (instructionForAll.Count > 0)
            {
                objRenderer.material.mainTexture = instructionForAll[currentSlide];
            }
        }
        else
        {
            Debug.LogError("Renderer component is missing on the InstructionPanel object.");
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
                GameObject player = GameObject.Find("OVRPlayerController");
                if (player != null)
                {
                    Transform plTransform = player.transform;
                    Vector3 plPosition = plTransform.position;
                    transform.position = plPosition + new Vector3(0f, 1.5f, 0f) + plTransform.forward * 1.5f;
                    transform.rotation = Quaternion.LookRotation(plTransform.forward, Vector3.up);
                }
                else
                {
                    Debug.LogWarning("OVRPlayerController not found!");
                }
            }
        }

        // Proceed to the next slide with the right trigger or the B key
        else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger) || Input.GetKeyDown(KeyCode.Space))
        {
            currentSlide++;
            List<Texture> instructions = GetRoleInstructionList();  // Get the correct list based on role

            if (currentSlide >= instructions.Count)
            {
                currentSlide = 0;  // Loop back to the first slide if necessary
            }

            objRenderer.material.mainTexture = instructions[currentSlide];
        }

        // Go back to the previous slide with the left trigger
        else if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger) || Input.GetKeyDown(KeyCode.B))
        {
            currentSlide--;
            List<Texture> instructions = GetRoleInstructionList();  // Get the correct list based on role

            if (currentSlide < 0)
            {
                currentSlide = instructions.Count - 1;  // Loop back to the last slide if necessary
            }

            objRenderer.material.mainTexture = instructions[currentSlide];
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
}

