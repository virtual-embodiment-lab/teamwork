using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VRNoPeeking : MonoBehaviour
{
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] float fadeSpeed;
    [SerializeField] float sphereCheckSize = .15f;
    [SerializeField] float threshold = 0.15f;
    [SerializeField] OVRPlayerController avatarController;

    private Material CameraFadeMat;
    private bool isCameraFadedOut = false;
    private Vector3 peekingPosition;

    private void Awake() => CameraFadeMat = GetComponent<Renderer>().material;

    // Update is called once per frame
    void Update()
    {
        if(Physics.CheckSphere(transform.position, sphereCheckSize, collisionLayer, QueryTriggerInteraction.Ignore)) 
        {
            if(!isCameraFadedOut)
            {
                isCameraFadedOut = true;
                //avatarController.EnableLinearMovement = false;
                //avatarController.EnableRotation = false;

                StartCoroutine(FadeCamera(true, 1f));
                //CameraFade(1f);
                peekingPosition = transform.position;
            }
        }
        else
        {
            if(!isCameraFadedOut)
                return;

            float dist = Vector3.Distance(peekingPosition, transform.position);
            if(dist < threshold)
            {
                isCameraFadedOut = false;
                //avatarController.EnableLinearMovement = true;
                //avatarController.EnableRotation = true;
                StartCoroutine(FadeCamera(false, 0f));
                //CameraFade(0f);
            }
                
        }
    }

    IEnumerator FadeCamera(bool FadedOut, float targetAlpha)
    {
        var fadeValue = Mathf.MoveTowards(CameraFadeMat.GetFloat("_AlphaValue"), targetAlpha, Time.deltaTime * fadeSpeed);
        while ((isCameraFadedOut && CameraFadeMat.GetFloat("_AlphaValue") <= targetAlpha) ||
                    (!isCameraFadedOut && CameraFadeMat.GetFloat("_AlphaValue") >= targetAlpha))
        {
            CameraFadeMat.SetFloat("_AlphaValue", fadeValue);
            fadeValue = Mathf.MoveTowards(CameraFadeMat.GetFloat("_AlphaValue"), targetAlpha, Time.deltaTime * fadeSpeed);

            yield return null;
        }
    }

    public void CameraFade(float targetAlpha)
    {
        var fadeValue = Mathf.MoveTowards(CameraFadeMat.GetFloat("_AlphaValue"), targetAlpha, Time.deltaTime * fadeSpeed);
        CameraFadeMat.SetFloat("_AlphaValue", fadeValue);

        if(fadeValue <= 0.01f)
            isCameraFadedOut = false;
    }
}
