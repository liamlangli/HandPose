using System.Collections.Generic;
using UnityEngine;
using ai;


public class ParaHandTracking : MonoBehaviour
{
    public enum ParaHandBone
    {   
        None = -1,
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
        PinkyTip,
    }

    private const int BoneCount = 15;
    
    private Renderer _renderer;
    public Animator animator;

    private Dictionary<ParaHandBone, int> _leftHandMap;
    private HumanBodyBones[] _rightHandBones;

    private HashSet<int> _skipBones;

    private List<LineRenderer> lines = new List<LineRenderer>();
    private ParaHandBone[,] lineBones = new ParaHandBone[5, 5];

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _leftHandMap = new Dictionary<ParaHandBone, int>();
        _leftHandMap[ParaHandBone.Wrist] = (int)HumanBodyBones.LeftHand;
        _leftHandMap[ParaHandBone.ThumbCMC] = (int)HumanBodyBones.LeftThumbProximal;
        _leftHandMap[ParaHandBone.ThumbMCP] = (int)HumanBodyBones.LeftThumbIntermediate;
        _leftHandMap[ParaHandBone.ThumbIP] = (int)HumanBodyBones.LeftThumbDistal;
        _leftHandMap[ParaHandBone.ThumbTip] = -1;
        _leftHandMap[ParaHandBone.IndexMCP] = (int)HumanBodyBones.LeftIndexProximal;
        _leftHandMap[ParaHandBone.IndexPIP] = (int)HumanBodyBones.LeftIndexIntermediate;
        _leftHandMap[ParaHandBone.IndexDIP] = (int)HumanBodyBones.LeftIndexDistal;
        _leftHandMap[ParaHandBone.IndexTip] = -1;
        _leftHandMap[ParaHandBone.MiddleMCP] = (int)HumanBodyBones.LeftMiddleProximal;
        _leftHandMap[ParaHandBone.MiddlePIP] = (int)HumanBodyBones.LeftMiddleIntermediate;
        _leftHandMap[ParaHandBone.MiddleDIP] = (int)HumanBodyBones.LeftMiddleDistal;
        _leftHandMap[ParaHandBone.MiddleTip] = -1;
        _leftHandMap[ParaHandBone.RingMCP] = (int)HumanBodyBones.LeftRingProximal;
        _leftHandMap[ParaHandBone.RingPIP] = (int)HumanBodyBones.LeftRingIntermediate;
        _leftHandMap[ParaHandBone.RingDIP] = (int)HumanBodyBones.LeftRingDistal;
        _leftHandMap[ParaHandBone.RingTip] = -1;
        _leftHandMap[ParaHandBone.PinkyMCP] = (int)HumanBodyBones.LeftLittleProximal;
        _leftHandMap[ParaHandBone.PinkyPIP] = (int)HumanBodyBones.LeftLittleIntermediate;
        _leftHandMap[ParaHandBone.PinkyDIP] = (int)HumanBodyBones.LeftLittleDistal;
        _leftHandMap[ParaHandBone.PinkyTip] = -1;
        
        rightHandBones = new HumanBodyBones[BoneCount]
        {
            HumanBodyBones.RightThumbProximal,
            HumanBodyBones.RightThumbIntermediate,
            HumanBodyBones.RightThumbDistal,
            HumanBodyBones.RightIndexProximal,
            HumanBodyBones.RightIndexIntermediate,
            HumanBodyBones.RightIndexDistal,
            HumanBodyBones.RightMiddleProximal,
            HumanBodyBones.RightMiddleIntermediate,
            HumanBodyBones.RightMiddleDistal,
            HumanBodyBones.RightRingProximal,
            HumanBodyBones.RightRingIntermediate,
            HumanBodyBones.RightRingDistal,
            HumanBodyBones.RightLittleProximal,
            HumanBodyBones.RightLittleIntermediate,
            HumanBodyBones.RightLittleDistal,
        };

        if (animator)
        {
            for (int i = 0; i < BONE_COUNT; i++)
            {
                leftHandRotations[i] = animator.GetBoneTransform(leftHandBones[i]).rotation;
                rightHandRotations[i] = animator.GetBoneTransform(rightHandBones[i]).rotation;
            }            
        }

        lineBones[0, 0] = ParaHandBone.Wrist;
        lineBones[0, 1] = ParaHandBone.ThumbCMC;
        lineBones[0, 2] = ParaHandBone.ThumbMCP;
        lineBones[0, 3] = ParaHandBone.ThumbIP;
        lineBones[0, 4] = ParaHandBone.ThumbTip;
        lineBones[1, 0] = ParaHandBone.Wrist;
        lineBones[1, 1] = ParaHandBone.IndexMCP;
        lineBones[1, 2] = ParaHandBone.IndexPIP;
        lineBones[1, 3] = ParaHandBone.IndexDIP;
        lineBones[1, 4] = ParaHandBone.IndexTip;
        lineBones[2, 0] = ParaHandBone.Wrist;
        lineBones[2, 1] = ParaHandBone.MiddleMCP;
        lineBones[2, 2] = ParaHandBone.MiddlePIP;
        lineBones[2, 3] = ParaHandBone.MiddleDIP;
        lineBones[2, 4] = ParaHandBone.MiddleTip;
        lineBones[3, 0] = ParaHandBone.Wrist;
        lineBones[3, 1] = ParaHandBone.RingMCP;
        lineBones[3, 2] = ParaHandBone.RingPIP;
        lineBones[3, 3] = ParaHandBone.RingDIP;
        lineBones[3, 4] = ParaHandBone.RingTip;
        lineBones[4, 0] = ParaHandBone.Wrist;
        lineBones[4, 1] = ParaHandBone.PinkyMCP;
        lineBones[4, 2] = ParaHandBone.PinkyPIP;
        lineBones[4, 3] = ParaHandBone.PinkyDIP;
        lineBones[4, 4] = ParaHandBone.PinkyTip;

        for (int i = 0; i < 5; ++i)
        {
            var lineObject = new GameObject(i.ToString());
            var line = lineObject.AddComponent<LineRenderer>();
            line.positionCount = 5;
            line.startWidth = line.endWidth = 0.002f;
            lines.Add(line);
        }
    }

    void Start()
    {
        ParaHandTrackingManager.instance.OnHandDetected += OnHandDetected;
        _renderer.material.mainTexture = ParaHandTrackingManager.instance.GetWebCamTexture();
    }

    private void Update()
    {
        ParaHandTrackingManager.instance.Predict();
    }

    private void OnHandDetected(ParaHand hand)
    {
        if (animator == null || hand.Score < 0.8) return;
        int index = 0;
        for (int i = 0; i < hand.Joints.Length; i++)
        {
            ParaHandBone bone = (ParaHandBone)i;
            if (prevBone.ContainsKey(bone) && nextBone.ContainsKey(bone))
            {
                ParaHandBone prev = prevBone[bone];
                ParaHandBone next = nextBone[bone];
                var prevDirection = (hand.Joints[i] - hand.Joints[(int)prev]).normalized;
                var nextDirection = (hand.Joints[(int)next] - hand.Joints[i]).normalized;
                boneRotations[index++].SetFromToRotation(prevDirection, nextDirection);
            }
        }

        Debug.Log($"Hand Score: {hand.Score}, Side Score: {hand.SideScore} {index}");
        
        var rightHand = hand.SideScore < 100;
        var bones = rightHand ? rightHandBones : leftHandBones;
        for (int i = 1; i < boneRotations.Length; i++)
        {
            var boneTransform = animator.GetBoneTransform(bones[i]);
            boneTransform.rotation = boneRotations[i];
        }
        
        // draw lines with hand joints
        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; j < 5; ++j) {
                lines[i].SetPosition(j, hand.Joints[(int)lineBones[i, j]]);
            }
        }
    }
}