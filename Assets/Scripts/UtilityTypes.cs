using System;
using System.Collections.Generic;
using StandardLogging;
using TMPro;
using UnityEngine;

namespace UtilityTypes
{
    [Serializable]
    public abstract class UtilityData
    {
        public UtilityData()
        {
        }
    }

    [Serializable]
    public class SequenceData
    {
        [SerializeField] public string step_name;
        [SerializeField] public GameObject UI_pre;
        [SerializeField] public GameObject UI_post;
        [SerializeField] public float duration_seconds;
        [SerializeField] public AvatarCreatorData avatarOverride;
        [SerializeField] public SlidesData slideOverride;

        public SequenceData()
        {
        }

        public override string ToString()
        {
            return $"Name:{step_name}, Duration Seconds:{duration_seconds}, Avatar Override:({avatarOverride}), Slide Override:({slideOverride})";
        }
    }

    [Serializable]
    public class SequenceManagerData
    {
        [SerializeField] public bool randomize;
        [SerializeField] public GameObject player;
        [SerializeField] public logmethod logMethod;
        [SerializeField] public GameObject KM_parent;
        [SerializeField] public GameObject VR_parent;
        [SerializeField] public GameObject UI_first;
        [SerializeField] public GameObject UI_controlls;
        [SerializeField] public GameObject UI_final;
        [SerializeField] public List<SequenceData> sequences;

        public SequenceManagerData()
        {
        }
    }

    [Serializable]
    public class AvatarCreatorData
    {
        [SerializeField] public float density;
        [SerializeField] public Vector3 positionOffset;
        [SerializeField] public Quaternion rotation;
        [SerializeField] public GameObject parent;
        [SerializeField] public bool createOne;
        [SerializeField] public int queueSize;
        [SerializeField] public float fadeSensitivity;
        [SerializeField] public bool fadeIn;
        [SerializeField] public bool fadeOut;
        [SerializeField] public bool showBars;
        [SerializeField] public List<GameObject> Avatar;

        public AvatarCreatorData()
        {
        }

        public override string ToString()
        {
            return $"Density:{density}, FadeIn:{fadeIn}, FadeOut:{fadeOut}, ShowBars:{showBars}";
        }
    }

    [Serializable]
    public class LoggerData
    {
        [SerializeField] public GameObject player;
        [SerializeField] public bool replay;
        [SerializeField] public int logFileSampleRate;
        [Header("Logger")]
        [SerializeField] public string trialName;
        [Header("Replay")]
        [SerializeField] public string logFilePath;
        //[SerializeField] public AudioWizard audioWizard;
        [SerializeField] public TMP_Text timeStampLive;
        [SerializeField] public List<AudioSource> Audio;

        public LoggerData()
        {
        }
    }

    [Serializable]
    public class SynchronyMonitorData
    {
        [SerializeField] public int logFileSampleRate;
        [SerializeField] public int bufferSize;
        [SerializeField] public TMP_Text headText;
        [SerializeField] public TMP_Text leftHandText;
        [SerializeField] public TMP_Text rightHandText;
        [SerializeField] public TMP_Text totalText;
        [SerializeField] public Color highSynchronyColor;
        [SerializeField] public Color lowSynchronyColor;
        [SerializeField] public List<MeshRenderer> materials; 

        public SynchronyMonitorData()
        {
        }
    }

    [Serializable]
    public class SlidesData
    {
        //[SerializeField] public SlideSequence slides;

        public SlidesData()
        {
        }

        public override string ToString()
        {
            return null;
            //return $"Name:{slides}";
        }
    }
}