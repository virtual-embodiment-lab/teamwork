// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRRigTransform
{
    public VRRigTransform()
    {
    }

    public VRRigTransform(Vector3 head, Vector3 leftHand, Vector3 rightHand)
    {
        Head = head;
        LeftHand = leftHand;
        RightHand = rightHand;
    }

    [field: SerializeField] public Vector3 Head;
    [field: SerializeField] public Vector3 LeftHand;
    [field: SerializeField] public Vector3 RightHand;
}

[System.Serializable]
public class VRRigTransformQueue
{
    public VRRigTransformQueue()
    {
      list = new List<VRRigTransform>();
    }

    public void Add(VRRigTransform vRRigTransform)
    {
        list.Add(vRRigTransform);
    }

    public Vector3 HeadDiff()
    {
        if (list.Count > 1)
        {
            return new Vector3(
                Mathf.Pow(list[list.Count-1].Head.x-list[list.Count-2].Head.x, 2),
                Mathf.Pow(list[list.Count-1].Head.y-list[list.Count-2].Head.y, 2),
                Mathf.Pow(list[list.Count-1].Head.z-list[list.Count-2].Head.z, 2));
        } else
        {
            return new Vector3(0, 0, 0);
        }
    }

    public Vector3 LPDiff()
    {
        if (list.Count > 1)
        {
            return new Vector3(
                Mathf.Pow(list[list.Count - 1].LeftHand.x - list[list.Count - 2].LeftHand.x, 2),
                Mathf.Pow(list[list.Count - 1].LeftHand.y - list[list.Count - 2].LeftHand.y, 2),
                Mathf.Pow(list[list.Count - 1].LeftHand.z - list[list.Count - 2].LeftHand.z, 2));
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    public Vector3 RPDiff()
    {
        if (list.Count > 1)
        {
            return new Vector3(
                Mathf.Pow(list[list.Count - 1].RightHand.x - list[list.Count - 2].RightHand.x, 2),
                Mathf.Pow(list[list.Count - 1].RightHand.y - list[list.Count - 2].RightHand.y, 2),
                Mathf.Pow(list[list.Count - 1].RightHand.z - list[list.Count - 2].RightHand.z, 2));
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    [field: SerializeField] public List<VRRigTransform> list;
}
