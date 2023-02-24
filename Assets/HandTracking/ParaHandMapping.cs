using System;
using System.Collections.Generic;
using UnityEngine;

namespace parahand {

public class ParahandMapping {

    public static Dictionary<ParaHandBone, Transform> Mapping(GameObject hand, bool isLeft) {
        var mapping = new Dictionary<ParaHandBone, Transform>();

        var prefix = "b_";
        prefix += isLeft ? "l_" : "r_";
        
        var armature = hand.transform.Find("Armature");

        mapping[ParaHandBone.Wrist] = armature.Find(prefix + "wrist");

        mapping[ParaHandBone.ThumbCMC] = armature.Find(prefix + "thumb1");
        mapping[ParaHandBone.ThumbMCP] = armature.Find(prefix + "thumb2");
        mapping[ParaHandBone.ThumbIP] = armature.Find(prefix + "thumb3");
        mapping[ParaHandBone.ThumbTip] = armature.Find(prefix + "thumb_null");

        mapping[ParaHandBone.IndexMCP] = armature.Find(prefix + "index1");
        mapping[ParaHandBone.IndexPIP] = armature.Find(prefix + "index2");
        mapping[ParaHandBone.IndexDIP] = armature.Find(prefix + "index3");
        mapping[ParaHandBone.IndexTip] = armature.Find(prefix + "index_null");

        mapping[ParaHandBone.MiddleMCP] = armature.Find(prefix + "middle1");
        mapping[ParaHandBone.MiddlePIP] = armature.Find(prefix + "middle2");
        mapping[ParaHandBone.MiddleDIP] = armature.Find(prefix + "middle3");
        mapping[ParaHandBone.MiddleTip] = armature.Find(prefix + "middle_null");

        mapping[ParaHandBone.RingMCP] = armature.Find(prefix + "ring1");
        mapping[ParaHandBone.RingPIP] = armature.Find(prefix + "ring2");
        mapping[ParaHandBone.RingDIP] = armature.Find(prefix + "ring3");
        mapping[ParaHandBone.RingTip] = armature.Find(prefix + "ring_null");

        mapping[ParaHandBone.PinkyMCP] = armature.Find(prefix + "pinky1");
        mapping[ParaHandBone.PinkyPIP] = armature.Find(prefix + "pinky2");
        mapping[ParaHandBone.PinkyDIP] = armature.Find(prefix + "pinky3");
        mapping[ParaHandBone.PinkyTip] = armature.Find(prefix + "pinky_null");

        mapping[ParaHandBone.Wrist] = armature;

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
        for (int i = 1; i < hand.Joints.Length; i++) {
            var bone = (ParaHandBone)i;
            if (bone == ParaHandBone.Wrist) continue;
            hand.Joints[i] = handInverse.MultiplyPoint3x4(hand.Joints[i]);
        }


        for (int i = 1; i < hand.Joints.Length; i++) {
            var bone = (ParaHandBone)i;
            if (skippedBones.Contains(bone)) continue;

            var prev = prevBone[bone];
            var next = nextBone[bone];

            var prevDirection = (hand.Joints[i] - hand.Joints[(int)prev]).normalized;
            var nextDirection = (hand.Joints[(int)next] - hand.Joints[i]).normalized;
            var boneDirection = Quaternion.FromToRotation(nextDirection, prevDirection).eulerAngles;

            var localEulerAngles = BoneEulerAngles[bone];
            localEulerAngles.z -= boneDirection.z;
            Bones[bone].localEulerAngles = localEulerAngles;
        }
    }

}

}