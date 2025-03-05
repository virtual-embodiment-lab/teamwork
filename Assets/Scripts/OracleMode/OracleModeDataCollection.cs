using Normal.Realtime;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


public class OracleModeDataCollection : MonoBehaviour
{
    [SerializeField] private List<GameObject> avatars;
    [SerializeField] private RealtimeAvatarManager avatarManager;
    [SerializeField] private TrackingTickSync trackingTickSync;
    [SerializeField] private StartTrackingSync startTrackingSync;
    [SerializeField] private int avatarCount;

    private Dictionary<int, int> receivedClientTicks = new Dictionary<int, int>();
    private StreamWriter tickTimeWriter;
    private bool isRecording = false;
    private int currentTick = 0;
    private float tickTimeout = 0.04f; // 40ms
    private float nextTickTime = 0;
    private int expectedClientCount = 0;
    [SerializeField] private string currentFilePath;
    [SerializeField] private string fileName;

    void Start()
    {
        avatars = new List<GameObject>();
        avatarManager = FindObjectOfType<RealtimeAvatarManager>();
        trackingTickSync = GetComponent<TrackingTickSync>();
        startTrackingSync = GetComponent<StartTrackingSync>();

        // Initialize the current tick and is recording
        currentTick = trackingTickSync.GetTrackingTick();
        isRecording = startTrackingSync.GetTracking();

        // Initialize the avatar list
        if (avatarManager != null)
        {
            avatarManager.avatarCreated += OnAvatarCreated;
            avatarManager.avatarDestroyed += OnAvatarDestroyed;
        }

        // Initialize the tick time writer
        fileName = $"oracle_mode_data_{DateTime.Now:yyyyMMdd_HHmmss}.tsv";
        currentFilePath = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log($"Saving file to: {currentFilePath}");
        tickTimeWriter = new StreamWriter(currentFilePath);
        tickTimeWriter.WriteLine("Tick\tServerTime\tLocalTime\tResponseCount\tExpectedCount");
    }

    void Update()
    {
        avatarCount = avatars.Count;

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleRecording();
        }

        if (isRecording && Time.time >= nextTickTime)
        {
            ProcessNextTick();
        }
    }

    void InitializeAvatarList()
    {
        foreach (RealtimeAvatar avatar in FindObjectsOfType<RealtimeAvatar>())
        {
            avatars.Add(avatar.gameObject);
        }
    }

    private void OnAvatarCreated(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        avatars.Add(avatar.gameObject);
        Debug.Log("Avatar created: " + avatar.gameObject.name);
        Debug.Log("Total avatars: " + avatars.Count);
    }

    private void OnAvatarDestroyed(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        avatars.Remove(avatar.gameObject);
        Debug.Log("Avatar destroyed: " + avatar.gameObject.name);
        Debug.Log("Total avatars: " + avatars.Count);
    }

    private void ToggleRecording()
    {
        isRecording = !isRecording;
        if (isRecording)
        {
            startTrackingSync.SetTracking(true);
            currentTick = 0;
            nextTickTime = Time.time;
            // Get current connected client count
            expectedClientCount = avatars.Count;
            Debug.Log($"Started recording with {expectedClientCount} clients");
        }
        else
        {
            startTrackingSync.SetTracking(false);
            if (tickTimeWriter != null)
            {
                tickTimeWriter.Close();
                AWSUploader.UploadFileToAWS(fileName, currentFilePath);
            }
        }
    }

    private void ProcessNextTick()
    {
        bool allClientsMatched = true;
        int matchedClients = 0;

        foreach (GameObject avatar in avatars)
        {
            ClientTickSync clientTick = avatar.GetComponent<ClientTickSync>();
            if (clientTick != null)
            {
                int clientCurrentTick = clientTick.GetCurrentTick();
                if (clientCurrentTick == currentTick)
                {
                    matchedClients++;
                }
                else
                {
                    allClientsMatched = false;
                }
            }
        }

        bool timeoutReached = (Time.time - nextTickTime) >= tickTimeout;

        if (allClientsMatched || timeoutReached)
        {
            if (!allClientsMatched)
            {
                Debug.LogWarning($"Tick {currentTick}: Only {matchedClients}/{expectedClientCount} clients matched");
            }

            // Record tick timing and response info
            string serverTime = DateTime.UtcNow.ToString("MM/dd/yy HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture);
            // string localTime = Time.time.ToString("MM/dd/yy HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture);
            string localTime = Time.time.ToString("F3");
            tickTimeWriter.WriteLine($"{currentTick}\t{serverTime}\t{localTime}\t{matchedClients}\t{expectedClientCount}");
            tickTimeWriter.Flush();

            // Move to next tick
            currentTick++;
            trackingTickSync.SetTrackingTick(currentTick);
            nextTickTime = Time.time;

            // Update expected client count in case clients joined/left
            expectedClientCount = avatars.Count;
        }
    }

    private async Task OnDestroy()
    {
        if (tickTimeWriter != null)
        {
            tickTimeWriter.Close();
            try
            {
                await AWSUploader.UploadFileToAWS(fileName, currentFilePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error uploading file to AWS: {e.Message}");
            }
        }
    }
}