// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

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


public class Logger_new : Utility
{
    [Header("Switch")]
    [SerializeField] bool log = false;
 
    [Header("Shared")]
    [SerializeField] public int logFileSampleRate = 10;
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
    public bool logActive = false;
    private StreamWriter writer = null;
    public bool gameStarted = false;
    private bool useRecordedPoses = false;

    [SerializeField] protected ValueMap<string, string> Tracked = new ValueMap<string, string>();
    [SerializeField] private string tempLoggedString = null;
    [SerializeField] private bool runLogging = true;

    //[SerializeField] public MicrophoneCapture mic = null;
    [SerializeField] public TMP_Text timeStampLive = null;


    internal void Setup(LoggerData loggerData)
    {
        log = true;
        runLogging = true;

        logFileSampleRate = loggerData.logFileSampleRate;
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
            // Get the RealtimeView component from the collider
            RealtimeView realtimeView = this.GetComponent<RealtimeView>();
            if (realtimeView != null)
            {
                // Check if the RealtimeView is owned by the local player
                if (realtimeView.isOwnedLocallySelf)
                {
                    if (!File.Exists(Application.persistentDataPath + "/trial_log_" + trialName + ".tsv"))
                    {
                        Debug.Log(" " + Application.persistentDataPath + "/trial_log_" + trialName + ".tsv");
                        logActive = true;
                        FileStream file = File.Open(Application.persistentDataPath + "/trial_log_" + trialName + ".tsv", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        writer = new StreamWriter(file);
                        writer.WriteLine("Player2");
                        writer.Flush();
                        putVarNames();
                        StartCoroutine("Logging");
                    }
                }
            }
        }
   }

    void putVarNames()
    {
        tempLoggedString = "TimeStamp" + "\t";
        tempLoggedString += "Event" + "\t";
        tempLoggedString += "roleInfo" + "\t";

        List<string> js = new List<string>{"leftJoystic", "rightJoystic"};
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

        List<string> nameLis = new List<string>{"hmd", "leftController", "rightController"};
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

    private void Update()
    {
        if (InputSystem.GetDevice<Keyboard>().eKey.wasPressedThisFrame ||
        (OVRInput.GetDown(OVRInput.RawButton.X) && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) == 1 && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) == 1) ||
        (OVRInput.GetDown(OVRInput.RawButton.A) && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) == 1 && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 1))
        {
            if (logActive)
            {
                StopCoroutine("Logging");
                logActive = false;
                writer.Close();
            }
        }
        
    }

    void FixedUpdate()
    {
        // Display Timestamp time in Update to keep it accurate + consistent
        //string currentTimestamp = "TimeStamp:" + Time.realtimeSinceStartup.ToString("0.00000") + "\t";
        //writer.WriteLine(currentTimestamp);
        //writer.Flush();
    }

    public void AddLine(string line)
    {
        DateTime dt = DateTime.Now;
        writer.WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + line);
        writer.Flush();
    }

    public void StartStopLog(bool logisRunning)
    {
        runLogging = logisRunning;
        if (logisRunning)
        {
            StartCoroutine("Logging");
        }
    }

    IEnumerator Logging()
    {
        while (runLogging)
        {
            DateTime dt = DateTime.Now;
            tempLoggedString = dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t";  //Time.realtimeSinceStartup.ToString("0.00000") + "\t";
            //timeStampLive.text = Time.realtimeSinceStartup.ToString("00.00");

            //space for events
            tempLoggedString += "\t"; 

            // info for each play
            Player myPlayer = GetComponent<Player>();

            switch (myPlayer.currentRole)
            {
                case Role.Collector:
                    //tempLoggedString += myPlayer.CurrentEnergy + "\t"; 
                    break;
                case Role.Explorer:
                    tempLoggedString += myPlayer.CurrentEnergy + "\t"; 
                    break;
                case Role.Tactical:
                    bool modeSwitch = GameObject.Find("switcher").GetComponent<contorllerSwitcher>().controllingAvatar;
                    if(!modeSwitch)
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
                Vector3 pos = obj.position;
                Quaternion rot = obj.rotation;
                tempLoggedString += pos.x + "\t" + pos.y + "\t" + pos.z + "\t";
                tempLoggedString += rot.x + "\t" + rot.y + "\t" + rot.z + "\t" + rot.w + "\t";
            }

            //record OVR info
            if(oculusIntegration)
            {
                List<Transform> objLis = new List<Transform>{headRig, leftHandRig, rightHandRig};
                foreach (Transform obj in LoggedObjects)
                {
                    Vector3 pos = obj.position;
                    Quaternion rot = obj.rotation;
                    tempLoggedString += pos.x + "\t" + pos.y + "\t" + pos.z + "\t";
                    tempLoggedString += rot.x + "\t" + rot.y + "\t" + rot.z + "\t" + rot.w + "\t";
                }
            }

            tempLoggedString += "\t";

            writer.WriteLine(tempLoggedString);
            writer.Flush();

            // Use non-scaled realtime method instead of scaled WaitForSeconds to count timestamp
            // Issue is that this gets called after Update, so it's closer to accurate but a few ms off potentially (>=)
            yield return new WaitForSecondsRealtime((float)(1.0 / logFileSampleRate));
        }
    }

    void OnApplicationQuit()
    {
        if (logActive)
        {
            StopCoroutine("Logging");
            StopCoroutine("Replaying");
            logActive = false;
            writer.Close();
        }
    }
}
