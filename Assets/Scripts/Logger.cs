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

public class Logger : Utility
{
    [Header("Switch")]
    [SerializeField] bool log = false;
    [SerializeField] bool replay = false;

    [Header("Shared")]
    [SerializeField] public int logFileSampleRate = 10;
    [SerializeField] List<Tracker> LoggedObjects = new List<Tracker>();

    [Header("Logger")]
    public string trialName = "test1";
    public bool logActive = false;
    private StreamWriter writer = null;
    public bool gameStarted = false;
    [SerializeField] protected ValueMap<string, string> Tracked = new ValueMap<string, string>();
    [SerializeField] private string tempLoggedString = null;
    [SerializeField] private bool runLogging = true;

    //[SerializeField] public MicrophoneCapture mic = null;
    [SerializeField] public TMP_Text timeStampLive = null;


    [Header("Replay")]
    [SerializeField] protected string logFilePath = null;
    [SerializeField] protected string[] logFile = null;
    [SerializeField] protected string text = " ";
    [SerializeField] public string timeStamp = "";
    [SerializeField] public string[] colonSplit;
    [SerializeField] protected string[] logSplit;
    [SerializeField] protected int currentLine = 0;
    //[SerializeField] protected AudioWizard audioWizard = null;

    //protected StreamReader reader = null;

    [SerializeField] public bool paused = false;
    [SerializeField] protected bool reversed = false;
    [SerializeField] protected int speed = 1;

    [SerializeField] public List<AudioSource> Audio = new List<AudioSource>();



    internal void Setup(LoggerData loggerData)
    {
        if (loggerData.replay)
        {
            replay = true;
            log = false;
        }
        else
        {
            replay = false;
            log = true;
            runLogging = true;
        }
        //mic = FindAnyObjectByType<MicrophoneCapture>();
        logFileSampleRate = loggerData.logFileSampleRate;
        trialName = loggerData.trialName;
        logFilePath = loggerData.logFilePath;
        Audio = loggerData.Audio;
        timeStampLive = loggerData.timeStampLive;
        //audioWizard = loggerData.audioWizard;
    }

    public override void Setup(UtilityData utilityData)
    {
        base.Setup(utilityData);
    }

    void Start()
    {
        if (log)
        {
            trialName = UnityEngine.Random.Range(0, 1000000).ToString();
            //mic.filename = "trial_log_" + trialName;
            gameStarted = true;

            foreach (var item in FindObjectsOfType<Tracker>())
            {
                LoggedObjects.Add(item);
            }

            // Logger
            if (!File.Exists(Application.persistentDataPath + "/trial_log_" + trialName + ".tsv"))
            {
                Debug.Log(" " + Application.persistentDataPath + "/trial_log_" + trialName + ".tsv");
                logActive = true;
                FileStream file = File.Open(Application.persistentDataPath + "/trial_log_" + trialName + ".tsv", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                writer = new StreamWriter(file);
                StartCoroutine("Logging");
            }
        }
        if (replay)
        {
            //logFile = Resources.Load(logFileString) as TextAsset;
            foreach (var item in FindObjectsOfType<Tracker>())
            {
                LoggedObjects.Add(item);
                item.StartReplayMode();
            }

            logFile = File.ReadAllLines(logFilePath);
            //reader = new StreamReader(new FileStream(logFilePath, FileMode.Open, FileAccess.ReadWrite));
            foreach (AudioSource audio in Audio)
            {
                audio.Play(0);
            }
            //InvokeRepeating("Replaying", 2.0f, 0.3f);
            StartCoroutine("Replaying");
        }

    }

    private void Update()
    {
        if (InputSystem.GetDevice<Keyboard>().eKey.wasPressedThisFrame ||
        (OVRInput.GetDown(OVRInput.RawButton.X) && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) == 1 && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) == 1) ||
        (OVRInput.GetDown(OVRInput.RawButton.A) && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) == 1 && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 1))
        { pausePlay(); }

        if (FindObjectsOfType<Tracker>().Length > LoggedObjects.Count)
        {
            foreach (var item in FindObjectsOfType<Tracker>())
            {
                if (!LoggedObjects.Contains(item))
                {
                    LoggedObjects.Add(item);
                }
            }
        }

        if (InputSystem.GetDevice<Keyboard>().eKey.wasPressedThisFrame ||
        (OVRInput.GetDown(OVRInput.RawButton.X) && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) == 1 && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) == 1) ||
        (OVRInput.GetDown(OVRInput.RawButton.A) && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) == 1 && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 1))
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

    void FixedUpdate()
    {
        // Display Timestamp time in Update to keep it accurate + consistent
        //string currentTimestamp = "TimeStamp:" + Time.realtimeSinceStartup.ToString("0.00000") + "\t";
        //writer.WriteLine(currentTimestamp);
        //writer.Flush();
    }

    public void AddLine(string line)
    {
        writer.WriteLine("TimeStamp:" + Time.realtimeSinceStartup.ToString("0.00000") + "\t" + line);
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
            tempLoggedString = "TimeStamp:" + Time.realtimeSinceStartup.ToString("0.00000") + "\t";
            timeStampLive.text = Time.realtimeSinceStartup.ToString("00.00");

            for (int i = 0; i < LoggedObjects.Count(); i++)
            {
                //Debug.Log(LoggedObjects);
                //Debug.Log(LoggedObjects.Count());

                foreach (KVPair<logtype, string> attributes in LoggedObjects[i].GetAttributes())
                {
                    Tracked.UpdateOrCreate(new KVPair<string, string>($"{i}:{attributes.Key}", attributes.Value));
                }
            }

            tempLoggedString += Tracked.ToLine('\t');

            writer.WriteLine(tempLoggedString);
            writer.Flush();

            // Use non-scaled realtime method instead of scaled WaitForSeconds to count timestamp
            // Issue is that this gets called after Update, so it's closer to accurate but a few ms off potentially (>=)
            yield return new WaitForSecondsRealtime((float)(1.0 / logFileSampleRate));
        }
    }

    public bool pausePlay()
    {
        return (paused = !paused);
    }

    public bool reversePlay()
    {
        return (reversed = !reversed);
    }

    public void resetPlay()
    {
        currentLine = 0;
    }

    public void endLinePlay()
    {
        currentLine = logFile.Length - 1;
    }

    public int setSpeed(int speed)
    {
        this.speed = speed;
        return speed;
    }

    IEnumerator Replaying()
    {
        while (true)
        {
            foreach (AudioSource audio in Audio)
            {
                if (paused)
                {
                    audio.UnPause();
                }
                else
                {
                    audio.Pause();
                }
            }

            while (!paused) yield return null;

            logSplit = logFile[currentLine].Split('\t');
            timeStamp = logSplit[0].Substring(10);

            //audioWizard.timeStamp = timeStamp;

            if (logSplit[1].Contains("Initiate step") || logSplit[1].Contains("Step Started"))
            {
                //TODO: Handle this case
                Debug.Log("Step Values");
            }
            else
            {
                for (int i = 1; i < logSplit.Length; i++)
                {
                    colonSplit = logSplit[i].Split(':');
                    if (colonSplit.Length > 1)
                    {
                        LoggedObjects[int.Parse(colonSplit[0])].ApplyValue(colonSplit[1], colonSplit[2]);
                    }
                }
            }
            currentLine = Math.Min(Math.Max(currentLine + (reversed ? -speed : speed), 0), logFile.Length - 1);

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
