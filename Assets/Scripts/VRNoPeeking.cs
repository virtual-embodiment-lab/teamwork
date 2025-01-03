// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System;
using System.Collections;
using System.Collections.Generic;
using UltimateXR.CameraUtils;
using UltimateXR.UI.UnityInputModule.Controls;
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
    private Vector3 prePosition;
    private Transform headPos;

    private void Awake()
    {
        headPos = GameObject.Find("CenterEyeAnchor").GetComponent<Transform>();
        prePosition = new Vector3(0.0f, 0.0f, 0.0f);
        CameraFadeMat = GetComponent<Renderer>().material;
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = headPos.position;
        if(Physics.CheckSphere(headPos.position, sphereCheckSize, collisionLayer, QueryTriggerInteraction.Ignore)) 
        {
            //Vector3 previousMovement  = (prePosition - currentPos).normalized*0.01f;
            avatarController.transform.position = prePosition;
        }
        prePosition = headPos.position;
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
