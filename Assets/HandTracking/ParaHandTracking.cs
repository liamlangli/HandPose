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

    public bool CameraPreview = true;

    private const int BoneCount = 15;

    private Dictionary<ParaHandBone, int> _leftHandMap;
    private HumanBodyBones[] _rightHandBones;

    private HashSet<int> _skipBones;

    private List<LineRenderer> lines = new List<LineRenderer>();
    private ParaHandBone[,] lineBones = new ParaHandBone[5, 5];

    private Texture _webCamTexture;

    private void Awake()
    {
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
        ParaHandTrackingManager.Instance.OnHandDetected += OnHandDetected;
        _webCamTexture = ParaHandTrackingManager.Instance.GetWebCamTexture();
    }

    private void Update()
    {
        ParaHandTrackingManager.Instance.Predict();

        if (CameraPreview && _webCamTexture) {
            Graphics.Blit(_webCamTexture, Camera.main.targetTexture);
        }
    }

    private void OnHandDetected(ParaHand hand)
    {        
        // draw lines with hand joints
        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; j < 5; ++j) {
                lines[i].SetPosition(j, hand.Joints[(int)lineBones[i, j]]);
            }
        }
    }
}