using Normal.Realtime;
using UltimateXR.Animation.Interpolation;
using UltimateXR.Audio;
using UnityEngine;


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
    [SerializeField] private UxrEasing _openEasing = UxrEasing.EaseOutCubic;
    [SerializeField] private UxrEasing _closeEasing = UxrEasing.EaseInCubic;
    [SerializeField] private UxrAudioSample _audioOpen;
    [SerializeField] private UxrAudioSample _audioClose;

    public float OpenValue { get; private set; }
    public bool IsOpen { get; private set; }

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
        }

        if (currentModel != null)
        {
            IsOpen = currentModel.isOpen;
            OpenValue = currentModel.isOpen ? 1.0f : 0.0f;

            // Register for events so we'll know if the isOpen property changes later
            currentModel.isOpenDidChange += IsOpenDidChange;
        }
    }

    private void IsOpenDidChange(DoorModel model, bool value)
    {
        // Update the door's state when the model changes
        IsOpen = value;
        OpenValue = value ? 1.0f : 0.0f;
    }

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

            if (distance < _openDistance && Mathf.Approximately(OpenValue, 0.0f))
            {
                _openDelayTimer += Time.deltaTime;

                if (_openDelayTimer > _openDelaySeconds && IsOpeningAllowed)
                {
                    // Within opening distance, door completely closed and opening allowed: open door
                    IsOpen = true;
                    model.isOpen = true;  // Make sure to update the model for Normcore synchronization
                    _audioOpen.Play(FloorCenter.position);
                }
            }
            else if (distance > _closeDistance && Mathf.Approximately(OpenValue, 1.0f))
            {
                // Over closing distance and door completely open: close door
                IsOpen = false;
                model.isOpen = false; // Make sure to update the model for Normcore synchronization
                _openDelayTimer = 0.0f;
                _audioClose.Play(FloorCenter.position);
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

    public void adjustOpeningDistance (float value)
    {
        _openDistance = value;
    }
}