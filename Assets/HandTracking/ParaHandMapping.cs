using System;
using System.Collections.Generic;
using UnityEngine;

namespace parahand {

public class ParahandMapping {

    public static Dictionary<ParaHandBone, Transform> Mapping(GameObject hand, bool isLeft) {
        var mapping = new Dictionary<ParaHandBone, Transform>();

        var side = isLeft ? "l" : "r";
        var prefix = "hands:b_";
        
        var root = hand.transform.Find("hands:" + side + "_hand_world");
        mapping[ParaHandBone.Wrist] = root.Find(prefix + side + "_hand");

        mapping[ParaHandBone.ThumbCMC] = mapping[ParaHandBone.Wrist].Find(prefix + side + "_thumb1");
        mapping[ParaHandBone.ThumbMCP] = mapping[ParaHandBone.ThumbCMC].Find(prefix + side + "_thumb2");
        mapping[ParaHandBone.ThumbIP]  = mapping[ParaHandBone.ThumbMCP].Find(prefix + side + "_thumb3");
        mapping[ParaHandBone.ThumbTip] = mapping[ParaHandBone.ThumbIP].Find(prefix + side + "_thumb_ignore");

        mapping[ParaHandBone.IndexMCP] = mapping[ParaHandBone.Wrist].Find(prefix + side + "_index1");
        mapping[ParaHandBone.IndexPIP] = mapping[ParaHandBone.IndexMCP].Find(prefix + side + "_index2");
        mapping[ParaHandBone.IndexDIP] = mapping[ParaHandBone.IndexPIP].Find(prefix + side + "_index3");
        mapping[ParaHandBone.IndexTip] = mapping[ParaHandBone.IndexDIP].Find(prefix + side + "_index_ignore");

        mapping[ParaHandBone.MiddleMCP] = mapping[ParaHandBone.Wrist].Find(prefix + side + "_middle1");
        mapping[ParaHandBone.MiddlePIP] = mapping[ParaHandBone.MiddleMCP].Find(prefix + side + "_middle2");
        mapping[ParaHandBone.MiddleDIP] = mapping[ParaHandBone.MiddlePIP].Find(prefix + side + "_middle3");
        mapping[ParaHandBone.MiddleTip] = mapping[ParaHandBone.MiddleDIP].Find(prefix + side + "_middle_ignore");

        mapping[ParaHandBone.RingMCP] = mapping[ParaHandBone.Wrist].Find(prefix + side + "_ring1");
        mapping[ParaHandBone.RingPIP] = mapping[ParaHandBone.RingMCP].Find(prefix + side + "_ring2");
        mapping[ParaHandBone.RingDIP] = mapping[ParaHandBone.RingPIP].Find(prefix + side + "_ring3");
        mapping[ParaHandBone.RingTip] = mapping[ParaHandBone.RingDIP].Find(prefix + side + "_ring_ignore");

        var pinky = mapping[ParaHandBone.Wrist].Find(prefix + side + "_pinky0");
        mapping[ParaHandBone.PinkyMCP] = pinky.Find(prefix + side + "_pinky1");
        mapping[ParaHandBone.PinkyPIP] = mapping[ParaHandBone.PinkyMCP].Find(prefix + side + "_pinky2");
        mapping[ParaHandBone.PinkyDIP] = mapping[ParaHandBone.PinkyPIP].Find(prefix + side + "_pinky3");
        mapping[ParaHandBone.PinkyTip] = mapping[ParaHandBone.PinkyDIP] .Find(prefix + side + "_pinky_ignore");

        return mapping;
    }
}

public enum ParaHandBone
{
    Wrist = 0,
    ThumbCMC,
    ThumbMCP,
    ThumbIP,
    ThumbTip,
    IndexMCP,
    IndexPIP,
    IndexDIP,
    IndexTip,
    MiddleMCP,
    MiddlePIP,
    MiddleDIP,
    MiddleTip,
    RingMCP,
    RingPIP,
    RingDIP,
    RingTip,
    PinkyMCP,
    PinkyPIP,
    PinkyDIP,
    PinkyTip
}

public class ParaHandModel {

    private Dictionary<ParaHandBone, ParaHandBone> nextBone = new Dictionary<ParaHandBone, ParaHandBone>();
    private Dictionary<ParaHandBone, ParaHandBone> prevBone = new Dictionary<ParaHandBone, ParaHandBone>();

    private HashSet<ParaHandBone> skippedBones = new HashSet<ParaHandBone> {
        ParaHandBone.Wrist,
        ParaHandBone.ThumbTip,
        ParaHandBone.IndexTip,
        ParaHandBone.MiddleTip,
        ParaHandBone.RingTip,
        ParaHandBone.PinkyTip
    };

    private HashSet<ParaHandBone> updateBones = new HashSet<ParaHandBone> {
        ParaHandBone.IndexMCP,
        ParaHandBone.MiddleMCP,
        ParaHandBone.RingMCP,
        ParaHandBone.PinkyMCP
    };

    public Vector3 Direction; // middle finger forward direction
    public bool IsLeft;
    public Dictionary<ParaHandBone, Transform> Bones = new Dictionary<ParaHandBone, Transform>();
    public Dictionary<ParaHandBone, Vector3> BoneEulerAngles = new Dictionary<ParaHandBone, Vector3>();

    public ParaHandModel(GameObject hand, bool isLeft) {
        Bones = ParahandMapping.Mapping(hand, isLeft);
        IsLeft = isLeft;

        nextBone[ParaHandBone.ThumbCMC] = ParaHandBone.ThumbMCP;
        nextBone[ParaHandBone.ThumbMCP] = ParaHandBone.ThumbIP;
        nextBone[ParaHandBone.ThumbIP] = ParaHandBone.ThumbTip;

        nextBone[ParaHandBone.IndexMCP] = ParaHandBone.IndexPIP;
        nextBone[ParaHandBone.IndexPIP] = ParaHandBone.IndexDIP;
        nextBone[ParaHandBone.IndexDIP] = ParaHandBone.IndexTip;

        nextBone[ParaHandBone.MiddleMCP] = ParaHandBone.MiddlePIP;
        nextBone[ParaHandBone.MiddlePIP] = ParaHandBone.MiddleDIP;
        nextBone[ParaHandBone.MiddleDIP] = ParaHandBone.MiddleTip;

        nextBone[ParaHandBone.RingMCP] = ParaHandBone.RingPIP;
        nextBone[ParaHandBone.RingPIP] = ParaHandBone.RingDIP;
        nextBone[ParaHandBone.RingDIP] = ParaHandBone.RingTip;

        nextBone[ParaHandBone.PinkyMCP] = ParaHandBone.PinkyPIP;
        nextBone[ParaHandBone.PinkyPIP] = ParaHandBone.PinkyDIP;
        nextBone[ParaHandBone.PinkyDIP] = ParaHandBone.PinkyTip;

        prevBone[ParaHandBone.ThumbCMC] = ParaHandBone.Wrist;
        prevBone[ParaHandBone.ThumbMCP] = ParaHandBone.ThumbCMC;
        prevBone[ParaHandBone.ThumbIP] = ParaHandBone.ThumbMCP;

        prevBone[ParaHandBone.IndexMCP] = ParaHandBone.Wrist;
        prevBone[ParaHandBone.IndexPIP] = ParaHandBone.IndexMCP;
        prevBone[ParaHandBone.IndexDIP] = ParaHandBone.IndexPIP;

        prevBone[ParaHandBone.MiddleMCP] = ParaHandBone.Wrist;
        prevBone[ParaHandBone.MiddlePIP] = ParaHandBone.MiddleMCP;
        prevBone[ParaHandBone.MiddleDIP] = ParaHandBone.MiddlePIP;

        prevBone[ParaHandBone.RingMCP] = ParaHandBone.Wrist;
        prevBone[ParaHandBone.RingPIP] = ParaHandBone.RingMCP;
        prevBone[ParaHandBone.RingDIP] = ParaHandBone.RingPIP;

        prevBone[ParaHandBone.PinkyMCP] = ParaHandBone.Wrist;
        prevBone[ParaHandBone.PinkyPIP] = ParaHandBone.PinkyMCP;
        prevBone[ParaHandBone.PinkyDIP] = ParaHandBone.PinkyPIP;

        var handZAxis = (Bones[ParaHandBone.MiddleMCP].position - Bones[ParaHandBone.Wrist].position).normalized;
        var handXAxis = (Bones[ParaHandBone.IndexMCP].position - Bones[ParaHandBone.PinkyMCP].position).normalized;
        Direction = Vector3.Cross(handZAxis, handXAxis).normalized;

        foreach (var bone in Bones) {
            BoneEulerAngles[bone.Key] = bone.Value.localEulerAngles;
        }
    }

    public void Update(ParaHand hand) {
        var wristPostition = hand.Joints[(int)ParaHandBone.Wrist];
        var handZAxis = (hand.Joints[(int)ParaHandBone.MiddleMCP] - wristPostition).normalized;
        var handXAxis = (hand.Joints[(int)ParaHandBone.IndexMCP] - hand.Joints[(int)ParaHandBone.PinkyMCP]).normalized;
        var handYAxis = Vector3.Cross(handZAxis, handXAxis).normalized;

        var handRotation = Quaternion.FromToRotation(Direction, handYAxis).normalized;
        Bones[ParaHandBone.Wrist].rotation = handRotation;

        var handInverse = Matrix4x4.TRS(wristPostition, Quaternion.LookRotation(handXAxis, handYAxis), Vector3.one).inverse;
        for (int i = 0; i < hand.Joints.Length; i++) {
            var bone = (ParaHandBone)i;
            hand.Joints[i] = handInverse.MultiplyPoint3x4(hand.Joints[i]);
        }

        for (int i = 0; i < hand.Joints.Length; i++) {
            var bone = (ParaHandBone)i;
            if (skippedBones.Contains(bone)) continue;

            var prev = prevBone[bone];
            var next = nextBone[bone];

            var prevDirection = (hand.Joints[i] - hand.Joints[(int)prev]).normalized;
            var nextDirection = (hand.Joints[(int)next] - hand.Joints[i]).normalized;
            var boneDirection = Quaternion.FromToRotation(nextDirection, prevDirection).eulerAngles;

            Debug.Log(boneDirection);

            var localEulerAngles = BoneEulerAngles[bone];
            localEulerAngles.z -= boneDirection.z;
            Bones[bone].localEulerAngles = localEulerAngles;
        }
    }

}

}