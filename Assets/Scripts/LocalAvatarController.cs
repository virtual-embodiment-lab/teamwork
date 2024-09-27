// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using Oculus.Interaction;
using Oculus.Interaction.Input;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalAvatarController : MonoBehaviour
{
    [SerializeField]
    private TrackedDataSource trackedDataSource;
    [SerializeField]
    private Transform avatarHandRoot_left;
    [SerializeField]
    private Transform avatarHandRoot_right;
 
    public Vector3 anchorOffset_left;
    public Vector3 anchorOffset_right;

    private List<Transform> jointID_to_trackedHandBoneSource_left = new List<Transform>();
    private List<Transform> jointID_to_trackedAvatarHandBone_left = new List<Transform>();

    private List<Transform> jointID_to_trackedHandBoneSource_right = new List<Transform>();
    private List<Transform> jointID_to_trackedAvatarHandBone_right = new List<Transform>();

    [SerializeField]
    private VRIK vRIK;

    public void init()
    {
        anchorOffset_left = new Vector3(-90, 0, 90);
        anchorOffset_right = new Vector3(90, 0, -90);
        //InitHandBoneMapping();
        UpdateIKTarget();
    }

    private void InitHandBoneMapping()
    {
        if (trackedDataSource == null || trackedDataSource.trackingActive == false)
            return;

        //HandWristRoot = HandStart + 0, // root frame of the hand, where the wrist is located
        //HandPinky3 = HandStart + 18, // pinky distal phalange bone

        for (int i = (int)HandJointId.HandWristRoot; i <= (int)HandJointId.HandPinky3; i++)
        {
            //HandForearmStub = HandStart + 1, // frame for user's forearm
            //HandThumb0 = HandStart + 2, // thumb trapezium bone
            //HandPinky0 = HandStart + 15, // pinky metacarpal bone
            if (i == 1 || i == 2 || i == 15)
                continue;

            //TODO-deal with those skipped bones better
            string fbxBoneName_left = Utils.OculusBoneNameFromHandJointId(Handedness.Left, (HandJointId)i);

            Transform fbxBoneSource_left = trackedDataSource.handVisualLeft.transform.FindChildRecursive(fbxBoneName_left);
            jointID_to_trackedHandBoneSource_left.Add(fbxBoneSource_left);

            string trackedAvatarBoneName_left = Utils.AvatarBoneNameFromHandJointId(Handedness.Left, (HandJointId)i);

            Transform trackedAvatarBone_left = avatarHandRoot_left.parent.FindChildRecursive(trackedAvatarBoneName_left);
            jointID_to_trackedAvatarHandBone_left.Add(trackedAvatarBone_left);


            string fbxBoneName_right = Utils.OculusBoneNameFromHandJointId(Handedness.Right, (HandJointId)i);

            Transform fbxBoneSource_right = trackedDataSource.handVisualRight.transform.FindChildRecursive(fbxBoneName_right);
            jointID_to_trackedHandBoneSource_right.Add(fbxBoneSource_right);

            string trackedAvatarBoneName_right = Utils.AvatarBoneNameFromHandJointId(Handedness.Right, (HandJointId)i);

            Transform trackedAvatarBone_right = avatarHandRoot_right.parent.FindChildRecursive(trackedAvatarBoneName_right);
            jointID_to_trackedAvatarHandBone_right.Add(trackedAvatarBone_right);

        }
    }

    public void UpdateBones()
    {
        //testing skip wrist
        for (int i = 0; i < jointID_to_trackedHandBoneSource_left.Count; i++)
        {
            jointID_to_trackedAvatarHandBone_left[i].rotation = jointID_to_trackedHandBoneSource_left[i].rotation;
            Quaternion rotOffset_left = Quaternion.Euler(anchorOffset_left);
            jointID_to_trackedAvatarHandBone_left[i].rotation *= rotOffset_left;

            jointID_to_trackedAvatarHandBone_right[i].rotation = jointID_to_trackedHandBoneSource_right[i].rotation;
            Quaternion rotOffset_right = Quaternion.Euler(anchorOffset_right);
            jointID_to_trackedAvatarHandBone_right[i].rotation *= rotOffset_right;

        }
    }

    public void UpdateIKTarget(bool useHandTracking = true)
    {
        if (trackedDataSource == null)
            return;

        if (vRIK.solver.spine.headTarget == null)
        {
            vRIK.solver.spine.headTarget = trackedDataSource.headAnchor;
        }

        if (vRIK.solver.leftArm.target == null)
        {
            vRIK.solver.leftArm.target = useHandTracking ? trackedDataSource.leftHandAnchor : trackedDataSource.leftControllerAnchor;
        }

        if (vRIK.solver.rightArm.target == null)
        {
            vRIK.solver.rightArm.target = useHandTracking ? trackedDataSource.rightHandAnchor : trackedDataSource.rightControllerAnchor;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        //If we use animated locomotion, we need to update the bones after the animtation
        UpdateBones();
    }
}
