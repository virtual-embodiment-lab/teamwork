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

    private int currentSlide  = 0;

    // Start is called before the first frame update
    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        objRenderer.enabled = showInstruction;

        // initialize panel with first image
        if (instructionForAll.Count > 0)
        {
            objRenderer.material.mainTexture = instructionForAll[currentSlide ];
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

                // Iterate through the textures in instructionForAll
                currentSlide++;
                if (currentSlide >= instructionForAll.Count)
                {
                    currentSlide = 0;  // Loop back to the first texture
                }

                // Update the texture on the panel
                objRenderer.material.mainTexture = instructionForAll[currentSlide];
            }  
        }
        else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger) || Input.GetKey(KeyCode.B))
        {
            GetComponent<Renderer>().material.mainTexture = instructionForAll[1];
                Debug.Log("test");
            }
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
