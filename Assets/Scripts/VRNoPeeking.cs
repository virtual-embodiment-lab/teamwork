// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class VRNoPeeking : MonoBehaviour
{
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] float fadeSpeed;
    [SerializeField] float sphereCheckSize = .15f;
    [SerializeField] float teleportSphereCheckSize = .55f;
    [SerializeField] float threshold = 0.15f;
    [SerializeField] Transform avatarController;

    private Material CameraFadeMat;
    private bool isCameraFadedOut = false;
    private Vector3 peekingPosition;
    private TeleportationProvider teleportationProvider;
    private Transform _xrOrigin;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 prePosition;
    private Transform headPos;

    private void Awake()
    {
        CameraFadeMat = GetComponent<Renderer>().material;
        _xrOrigin = transform.root;
        headPos = GameObject.Find("Main Camera").GetComponent<Transform>();
        prePosition = new Vector3(0.0f, 0.0f, 0.0f);
        teleportationProvider = FindObjectOfType<TeleportationProvider>();
        if (teleportationProvider != null)
        {
            teleportationProvider.beginLocomotion += OnTeleportStart;
            teleportationProvider.endLocomotion += OnTeleportEnd;
        }
    }

    // Update is called once per frame
    void Update()
    { 
        Vector3 currentPos = headPos.position;
        if(Physics.CheckSphere(headPos.position, sphereCheckSize, collisionLayer, QueryTriggerInteraction.Ignore)) 
        {
            _xrOrigin.transform.position = prePosition;
        }
        prePosition = headPos.position;
    }

    private void OnTeleportStart(LocomotionSystem locomotionSystem)
    {
        startPosition = _xrOrigin.position;
    }

    private void OnTeleportEnd(LocomotionSystem locomotionSystem)
    {
        endPosition = _xrOrigin.position;
        Vector3 direction = (endPosition - startPosition).normalized;
        while (Physics.CheckSphere(transform.position, teleportSphereCheckSize, collisionLayer, QueryTriggerInteraction.Ignore)) 
        {
            GameObject[] avatars = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject avatar in avatars)
            {
                UIManager uiManager = avatar.GetComponent<UIManager>();
                //uiManager.ShowMessage($"{direction.ToString()}");
            }
            _xrOrigin.position = _xrOrigin.position - direction;
        }
    }

    public void FadeForTeleport(float delay)
    {
        //StartCoroutine(FadeCamera(false, 0f));

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
