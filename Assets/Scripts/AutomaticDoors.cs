// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using Normal.Realtime;
using UltimateXR.Animation.Interpolation;
using UltimateXR.Audio;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;

public class AutomaticDoor : RealtimeComponent<DoorModel>
{
    [SerializeField] private SpawnPoint assignedSpawnPoint;
    [SerializeField] private Realtime _realtime;
    [SerializeField] private OVRPlayerController _ovrPlayerController;
    [SerializeField] private Transform _floorCenter;
    [SerializeField] private Transform _leftDoor;
    [SerializeField] private Transform _rightDoor;
    [SerializeField] private Vector3 _leftOpenLocalOffset;
    [SerializeField] private Vector3 _rightOpenLocalOffset;
    [SerializeField] private float _openDelaySeconds;
    [SerializeField] private float _openDurationSeconds = 0.8f;
    [SerializeField] private float _openDistance = 1.5f;
    [SerializeField] private float _closeDistance = 2.0f;
    [SerializeField] private Role _playerFlag;
    [SerializeField] private UxrEasing _openEasing = UxrEasing.EaseOutCubic;
    [SerializeField] private UxrEasing _closeEasing = UxrEasing.EaseInCubic;
    [SerializeField] private UxrAudioSample _audioOpen;
    [SerializeField] private UxrAudioSample _audioClose;

    public float OpenValue { get; private set; }
    public bool IsOpen { get; private set; }
    public List<int> OpenPlayer = new List<int>();

    public void OpenDoor()
    {
        model.isOpen = true; // This will automatically sync with all clients.
    }

    protected override void OnRealtimeModelReplaced(DoorModel previousModel, DoorModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events on the previous model
            previousModel.isOpenDidChange -= IsOpenDidChange;
            previousModel.openerDidChange -= OpenerDidChange;
        }

        if (currentModel != null)
        {
            // Register for events so we'll know if the isOpen property changes later
            currentModel.isOpenDidChange += IsOpenDidChange;
            previousModel.openerDidChange += OpenerDidChange;
        }
    }

    private void IsOpenDidChange(DoorModel model, bool value)
    {
        // Update the door's state when the model changes
        Debug.Log("isOpen is changed"+value);
        IsOpen = value;
        OpenValue = value ? 1.0f : 0.0f;
        Debug.Log("openValue: "+OpenValue);
    }

   private void OpenerDidChange(DoorModel model, int value)
    {
        if (value < 0)
        {
            if (OpenPlayer.Contains(math.abs(value)))
            {
                OpenPlayer.Remove(math.abs(value));
            }
        }else{
            if (!OpenPlayer.Contains(value))
            {
                OpenPlayer.Add(value);
            }
        }
    }

    public void distanceChange(float value) => _openDistance = value;

    protected void Awake()
    {
        _leftStartLocalPosition = _leftDoor.localPosition;
        _rightStartLocalPosition = _rightDoor.localPosition;
    }

    private void Update()
    {
        if (_realtime == null || _ovrPlayerController == null)
        {
            Debug.LogWarning("Realtime or OVRPlayerController component is not assigned to the AutomaticDoor");
            return;
        }

        int localAvatarClientID = _realtime.clientID;

        bool isAvatarAllowedThroughDoor = (assignedSpawnPoint == null ||
                                            assignedSpawnPoint.GetOccupyingClientID() == localAvatarClientID);

        if (isAvatarAllowedThroughDoor)
        {
            // Use the position of the OVRPlayerController instead of UxrAvatar
            Vector3 playerPosition = _ovrPlayerController.transform.position;

            // Check distance to door
            float distance = Vector3.Distance(playerPosition, FloorCenter.position);

            // Door is closed 
            if (distance < _openDistance && Mathf.Approximately(OpenValue, 0.0f))
            {

                GameObject[] avatars = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject avatar in avatars)
                {
                    RealtimeView realtimeView = avatar.GetComponent<RealtimeView>();

                    if (realtimeView != null && realtimeView.isOwnedLocallySelf)
                    {
                        if (_playerFlag == Role.None || avatar.GetComponent<Player>().currentRole == _playerFlag)
                        {
                            _openDelayTimer += Time.deltaTime;

                            if (_openDelayTimer > _openDelaySeconds && IsOpeningAllowed)
                            {
                                // Within opening distance, door completely closed and opening allowed: open door
                                IsOpen = true;
                                model.isOpen = true;  // Make sure to update the model for Normcore synchronization
                                OpenPlayer.Add(localAvatarClientID);
                                model.opener = localAvatarClientID;
                                _audioOpen.Play(FloorCenter.position);
                            }
                        }
                    }
                }
            }
            else if (distance > _closeDistance && Mathf.Approximately(OpenValue, 1.0f))
            {
                if (OpenPlayer.Contains(localAvatarClientID))
                {
                    OpenPlayer.Remove(localAvatarClientID);
                    model.opener = -localAvatarClientID;

                    // If there are other door open plyers
                    if(OpenPlayer.Count == 0)
                    {
                        // Over closing distance and door completely open: close door
                        IsOpen = false;
                        model.isOpen = false; // Make sure to update the model for Normcore synchronization
                        _openDelayTimer = 0.0f;
                        _audioClose.Play(FloorCenter.position);
                        Debug.Log("close");
                    }
                }
            }

            // Update timer and perform interpolation
            OpenValue = Mathf.Clamp01(OpenValue + Time.deltaTime * (1.0f / _openDurationSeconds) * (IsOpen ? 1.0f : -1.0f));
            float t = UxrInterpolator.GetInterpolationFactor(OpenValue, IsOpen ? _openEasing : _closeEasing);

            _leftDoor.transform.localPosition = Vector3.Lerp(_leftStartLocalPosition, _leftStartLocalPosition + _leftOpenLocalOffset, t);
            _rightDoor.transform.localPosition = Vector3.Lerp(_rightStartLocalPosition, _rightStartLocalPosition + _rightOpenLocalOffset, t);
        }
    }

    protected virtual bool IsOpeningAllowed => true;

    protected Transform FloorCenter => _floorCenter != null ? _floorCenter : transform;

    private float _openDelayTimer;
    private Vector3 _leftStartLocalPosition;
    private Vector3 _rightStartLocalPosition;
}