// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandAnimationController : MonoBehaviour
{

    public InputDeviceCharacteristics controllerType;
    private InputDevice _thisController;

    public Animator _animationController;
    private bool _isControllerFound;

    // Start is called before the first frame update
    void Start()
    {
        _animationController = GetComponent<Animator>();
        Initialise();

    }

    private void Initialise() {
        List<InputDevice> xrDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerType, xrDevices);

        if (xrDevices.Count.Equals(0)) {
            Debug.Log("No XR Devices");
        }
        else
        {
            _thisController = xrDevices[0];
            _isControllerFound = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!_isControllerFound)
        {
            Initialise();
        }
        else
        {
            if(_thisController.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            {
                _animationController.SetFloat("Trigger", triggerValue);

            }

            if (_thisController.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                _animationController.SetFloat("Grip", gripValue);

            }
        }

    }
}
