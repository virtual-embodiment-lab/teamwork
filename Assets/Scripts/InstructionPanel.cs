using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionPanel : MonoBehaviour
{
    //[SerializeField] private Material mat;
    private bool showInstruction = false;
    private Renderer objRenderer;

    // Start is called before the first frame update
    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        objRenderer.enabled = showInstruction;
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
    }
}
