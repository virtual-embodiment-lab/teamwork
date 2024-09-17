using System;
using System.Collections.Generic;
using StandardLogging;
using UnityEngine;

[DisallowMultipleComponent]
/* Contains a list of attributes to be tracked and saved to a log file.
 * These will enable the replaying of these elements. The Logger will go
 * through all of these classes and get the values in the Attributes list
 * every log cycle. We are storing the Items in a list to allow Unity to
 * serialize the data but are using a Dictionary to map the indexes for
 * fast retreival */
public abstract class Tracker : MonoBehaviour
{
    [Header("Tracker")]
    [EnumFlags] public logtype m_options;
    [SerializeField] protected ValueMap<logtype, string> Map = new ValueMap<logtype, string>();

    public virtual void StartReplayMode() { }

    //public void StartReplayMode()
    //{
    //    MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
    //    foreach (MonoBehaviour c in comps)
    //    {
    //        if (c.GetType() != typeof(Tracker))
    //        {
    //            c.enabled = false;
    //        }
    //    }
    //}

    /* Add or Update value on the value map */
    protected void UpdateOrCreate(KVPair<logtype, string> input)
    {
        Map.UpdateOrCreate(input);
    }

    /* Returns the list of attributes */
    public List<KVPair<logtype, string>> GetAttributes()
    {
        return Map.GetAttributes();
    }

    public virtual void ApplyValue(string type, string value)
    {
        if ((logtype)Enum.Parse(typeof(logtype), type) == logtype.Position)
            transform.position = parseVector3(value);
        if ((logtype)Enum.Parse(typeof(logtype), type) == logtype.Rotation)
            transform.localEulerAngles = parseVector3(value);
        if ((logtype)Enum.Parse(typeof(logtype), type) == logtype.Scale)
            transform.localScale = parseVector3(value);
    }

    /* Keep Attributes up to date with values */
    protected virtual void Update()
    {
        List<string> logList = EnumFlagsAttribute.GetSelectedStrings(m_options);
        if (logList.Contains(logtype.Position.ToString()))
        {
            Map.UpdateOrCreate(new KVPair<logtype, string>(logtype.Position, transform.localPosition.ToString()));
        }
        if (logList.Contains(logtype.Rotation.ToString()))
        {
            Map.UpdateOrCreate(new KVPair<logtype, string>(logtype.Rotation, transform.localEulerAngles.ToString()));
        }
        if (logList.Contains(logtype.Scale.ToString()))
        {
            Map.UpdateOrCreate(new KVPair<logtype, string>(logtype.Scale, transform.localScale.ToString()));
        }
    }

    protected Vector3 parseVector3(string sourceString)
    {
        string outString;
        Vector3 outVector3;
        string[] splitString;
        outString = sourceString.Substring(1, sourceString.Length - 2);
        splitString = outString.Split(","[0]);
        outVector3.x = float.Parse(splitString[0]);
        outVector3.y = float.Parse(splitString[1]);
        outVector3.z = float.Parse(splitString[2]);
        return outVector3;
    }

    protected Quaternion parseQuaternion(string sourceString)
    {
        string outString;
        string[] splitString;
        outString = sourceString.Substring(1, sourceString.Length - 2);
        splitString = outString.Split(","[0]);
        return new Quaternion(float.Parse(splitString[0]), float.Parse(splitString[1]), float.Parse(splitString[2]), float.Parse(splitString[3]));
    }
}
