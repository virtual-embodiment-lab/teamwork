using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StandardLogging;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityTypes;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using Oculus.VoiceSDK.UX;
using Unity.Mathematics;
using Unity.VisualScripting;
using System.Xml;
using Normal.Realtime;
using System.Threading.Tasks;


public class ClientDataLogger : Utility
{
    [Header("Switch")]
    [SerializeField] bool log = true;

    [Header("Shared")]
    [SerializeField] public int logFileSampleRate = 50;
    [SerializeField] public List<Transform> LoggedObjects = new List<Transform>();
    public Transform head = null;
    public Transform leftHand = null;
    public Transform rightHand = null;

    [SerializeField] private bool oculusIntegration = true;

    [SerializeField] private Transform headRig = null;
    [SerializeField] private Transform leftHandRig = null;
    [SerializeField] private Transform rightHandRig = null;

    [SerializeField] private Transform rig = null;
    [SerializeField] private OVRCameraRig ovrRig = null;
    [SerializeField] private OVRCameraRigRef ovrRigRef = null;

    [Header("Logger")]
    public string trialName = "test1";
    private StreamWriter writer = null;
    private bool gameStarted = false;
    private bool useRecordedPoses = false;

    // New fields for tick-based logging
    [SerializeField] private ClientTickSync clientTickSync;
    [SerializeField] private TrackingTickSync trackingTickSync;
    [SerializeField] private StartTrackingSync startTrackingSync;
    [SerializeField] private OracleModeDataCollection oracleModeDataCollection;
    private int lastRecordedTick = -1;
    [SerializeField] private string filePath = "";
    [SerializeField] private string fileName = "";
    [SerializeField] private string tempLoggedString = null;
    [SerializeField] public TMP_Text timeStampLive = null;
    private Coroutine loggingCoroutine = null;
    private bool isLogging = false;
    [SerializeField] private RealtimeView realtimeView;


    internal void Setup(LoggerData loggerData)
    {
        log = true;
        trialName = loggerData.trialName;
        timeStampLive = loggerData.timeStampLive;
    }

    public override void Setup(UtilityData utilityData)
    {
        base.Setup(utilityData);
    }

    private void Init()
    {
        if (oculusIntegration)
        {
            ovrRig = FindObjectOfType<OVRCameraRig>(true); //both active and inactive objects
            if (!useRecordedPoses)
            {
                if (ovrRig != null)
                {
                    ovrRigRef = FindObjectOfType<OVRCameraRigRef>(true);
                    headRig = transform.Find("ovrRigRef/OVRInteraction/OVRHmd");
                    leftHandRig = transform.Find("ovrRigRef/OVRInteraction/OVRControllerHands/LeftControllerHand");
                    rightHandRig = transform.Find("ovrRigRef/OVRInteraction/OVRControllerHands/RightControllerHand");
                }
            }
        }
    }

    void Start()
    {
        if (log)
        {
            trialName = UnityEngine.Random.Range(0, 1000000).ToString();
            gameStarted = true;
            Init();

            // Logger
            realtimeView = GetComponent<RealtimeView>();
            if (realtimeView != null && realtimeView.isOwnedLocallySelf)
            {
                // Initialize tick sync components
                clientTickSync = GetComponent<ClientTickSync>();
                if (clientTickSync == null)
                {
                    clientTickSync = gameObject.AddComponent<ClientTickSync>();
                }

                GameObject oracleManager = GameObject.Find("Oracle Manager");
                if (oracleManager == null)
                {
                    Debug.LogError("oracleManager not found in scene");
                }

                trackingTickSync = oracleManager.GetComponent<TrackingTickSync>();
                startTrackingSync = oracleManager.GetComponent<StartTrackingSync>();
                if (!File.Exists(Application.persistentDataPath + "/player_log_" + trialName + ".tsv"))
                {
                    Debug.Log(" " + Application.persistentDataPath + "/player_log_" + trialName + ".tsv");
                    FileStream file = File.Open(Application.persistentDataPath + "/player_log_" + trialName + ".tsv", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    filePath = Application.persistentDataPath + "/player_log_" + trialName + ".tsv";
                    fileName = "player_log_" + trialName + ".tsv";
                    writer = new StreamWriter(file);
                    writer.WriteLine("Player" + realtimeView.ownerID);
                    writer.Flush();
                    putVarNames();
                }
            }
        }
    }

    private void putVarNames()
    {
        tempLoggedString = "Tick" + "\t";
        tempLoggedString += "Event" + "\t";
        tempLoggedString += "roleInfo" + "\t";

        List<string> js = new List<string> { "leftJoystic", "rightJoystic" };
        foreach (string str in js)
        {
            tempLoggedString += str + ".x\t";
            tempLoggedString += str + ".y\t";
        }

        tempLoggedString += "position:root" + ".x\t";
        tempLoggedString += "position:root" + ".y\t";
        tempLoggedString += "position:root" + ".z\t";
        tempLoggedString += "rotation:root" + ".x\t";
        tempLoggedString += "rotation:root" + ".y\t";
        tempLoggedString += "rotation:root" + ".z\t";
        tempLoggedString += "rotation:root" + ".w\t";

        foreach (Transform obj in LoggedObjects)
        {
            string name = obj.name;
            tempLoggedString += "position:" + name + ".x\t";
            tempLoggedString += "position:" + name + ".y\t";
            tempLoggedString += "position:" + name + ".z\t";
            tempLoggedString += "rotation:" + name + ".x\t";
            tempLoggedString += "rotation:" + name + ".y\t";
            tempLoggedString += "rotation:" + name + ".z\t";
            tempLoggedString += "rotation:" + name + ".w\t";

        }

        List<string> nameLis = new List<string> { "hmd", "leftController", "rightController" };
        if (oculusIntegration)
        {
            foreach (string str in nameLis)
            {
                tempLoggedString += "position:" + str + ".x\t";
                tempLoggedString += "position:" + str + ".y\t";
                tempLoggedString += "position:" + str + ".z\t";
                tempLoggedString += "rotation:" + str + ".x\t";
                tempLoggedString += "rotation:" + str + ".y\t";
                tempLoggedString += "rotation:" + str + ".z\t";
                tempLoggedString += "rotation:" + str + ".w\t";
            }
        }
        writer.WriteLine(tempLoggedString);
        writer.Flush();
    }

    void Update()
    {
        if (!realtimeView.isOwnedLocallySelf)
        {
            realtimeView.RequestOwnership();
            return;
        }

        if (writer == null) return;

        // Start logging when tracking begins
        if (startTrackingSync.GetTracking() && !isLogging)
        {
            StartLogging();
        }
        // Stop logging when tracking ends
        else if (!startTrackingSync.GetTracking() && isLogging)
        {
            StopLogging();
        }
    }

    public void AddLine(string line)
    {
        DateTime dt = DateTime.Now;
        writer.WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + line);
        writer.Flush();
    }

    private void StartLogging()
    {
        isLogging = true;
        loggingCoroutine = StartCoroutine(Logging());
    }

    private void StopLogging()
    {
        if (loggingCoroutine != null)
        {
            StopCoroutine(loggingCoroutine);
            loggingCoroutine = null;
        }
        isLogging = false;
        if (writer != null)
        {
            writer.Close();
            AWSUploader.UploadFileToAWS(fileName, filePath);
        }
    }

    IEnumerator Logging()
    {
        while (true)
        {
            if (!realtimeView.isOwnedLocallySelf)
            {
                realtimeView.RequestOwnership();
                continue;
            }
            // Get current tick from Oracle
            int currentTick = trackingTickSync.GetTrackingTick();

            // Only record if we haven't recorded this tick yet
            if (currentTick > lastRecordedTick)
            {
                RecordData(currentTick);
                lastRecordedTick = currentTick;

                // Update our client's tick to show we've recorded this tick
                clientTickSync.SetCurrentTick(currentTick);
            }

            yield return new WaitForSecondsRealtime((float)(1.0 / logFileSampleRate));
        }
    }

    private void RecordData(int tick)
    {
        tempLoggedString = tick.ToString() + "\t";
        //space for events
        tempLoggedString += "\t";

        // info for each play
        Player myPlayer = GetComponent<Player>();

        switch (myPlayer.currentRole)
        {
            case Role.Collector:
                break;
            case Role.Explorer:
                tempLoggedString += myPlayer.CurrentEnergy + "\t";
                break;
            case Role.Tactical:
                bool modeSwitch = GameObject.Find("switcher").GetComponent<contorllerSwitcher>().controllingAvatar;
                if (!modeSwitch)
                {
                    Transform tacticalCam = GameObject.Find("tacticalView").GetComponent<Transform>();
                    tempLoggedString += tacticalCam.position + "\t";
                }
                break;
            default:
                break;
        }
        tempLoggedString += "\t";

        //record joystick angle
        // rleft controller
        float horizontalL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        float verticalL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;
        // right controller
        float horizontalR = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
        float verticalR = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;
        tempLoggedString += horizontalL + "\t" + verticalL + "\t" + horizontalR + "\t" + verticalR + "\t";

        //record avatar's info
        Vector3 p = this.transform.position;
        Quaternion r = this.transform.rotation;
        tempLoggedString += p.x + "\t" + p.y + "\t" + p.z + "\t";
        tempLoggedString += r.x + "\t" + r.y + "\t" + r.z + "\t" + r.w + "\t";

        //record logged objects' position and rotation
        foreach (Transform obj in LoggedObjects)
        {
            Vector3 pos = obj.localPosition;
            Quaternion rot = obj.localRotation;
            tempLoggedString += pos.x + "\t" + pos.y + "\t" + pos.z + "\t";
            tempLoggedString += rot.x + "\t" + rot.y + "\t" + rot.z + "\t" + rot.w + "\t";
        }

        //record OVR info
        if (oculusIntegration)
        {
            List<Transform> objLis = new List<Transform> { headRig, leftHandRig, rightHandRig };
            foreach (Transform obj in LoggedObjects)
            {
                Vector3 pos = obj.localPosition;
                Quaternion rot = obj.localRotation;
                tempLoggedString += pos.x + "\t" + pos.y + "\t" + pos.z + "\t";
                tempLoggedString += rot.x + "\t" + rot.y + "\t" + rot.z + "\t" + rot.w + "\t";
            }
        }

        tempLoggedString += "\t";

        writer.WriteLine(tempLoggedString);
        writer.Flush();
    }

    private void OnDisable()
    {
        StopLogging();
    }

    private async Task OnDestroy()
    {
        if (writer != null)
        {
            writer.Close();
            await AWSUploader.UploadFileToAWS(fileName, filePath);
        }
    }

}