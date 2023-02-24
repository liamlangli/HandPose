using System.Collections.Generic;
using UnityEngine;

namespace parahand {

public class ParaHandTracking : MonoBehaviour
{
    public GameObject CameraPreview;
    public GameObject LeftHand;
    public GameObject RightHand;

    [Range(0, 1)]
    public float LeftHandSmoothFactor = 0.7f;

    [Range(0, 1)]
    public float RightHandSmoothFactor = 0.7f;

    private List<LineRenderer> lines = new List<LineRenderer>();
    private ParaHandBone[,] lineBones = new ParaHandBone[5, 5];

    private Renderer _renederer;

    private ParaHandModel _leftHandModel;
    private ParaHandModel _rightHandModel;

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

        _renederer = CameraPreview.GetComponent<Renderer>();

        if (LeftHand) {
            _leftHandModel = new ParaHandModel(LeftHand, true);
            _leftHandModel.LerpFactor = LeftHandSmoothFactor;
            LeftHand.SetActive(false);
        }
        if (RightHand) {
            _rightHandModel = new ParaHandModel(RightHand, false);
            _rightHandModel.LerpFactor = RightHandSmoothFactor;
            RightHand.SetActive(false);
        }
    }

    void Start()
    {
        ParaHandTrackingManager.Instance.OnHandDetected += OnHandDetected;
        if (_renederer) {
            _renederer.material.mainTexture = ParaHandTrackingManager.Instance.GetWebCamTexture();
        }
    }

    private void Update()
    {
        ParaHandTrackingManager.Instance.Predict();
    }

    private void OnHandDetected(ParaHand hand)
    {
        if (hand.Score < 0.2) {
            if (LeftHand) {
                LeftHand.SetActive(false);
            }
            if (RightHand) {
                RightHand.SetActive(false);
            }
            return;
        }

        if (hand.IsLeft) {
            if (LeftHand) {
                LeftHand.SetActive(true);
                _leftHandModel.Update(hand);
            }
            if (RightHand) {
                RightHand.SetActive(false);
            }
        } else {
            if (RightHand) {
                RightHand.SetActive(true);
                _rightHandModel.Update(hand);
            }
            if (LeftHand) {
                LeftHand.SetActive(false);
            }
        }

        // draw lines with hand joints
        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; j < 5; ++j) {
                lines[i].SetPosition(j, hand.Joints[(int)lineBones[i, j]] + transform.position);
            }
        }
    }
    
    private void OnVaidate() {
        if (_renederer) _renederer.enabled = CameraPreview;
        if (_leftHandModel != null) _leftHandModel.LerpFactor = LeftHandSmoothFactor;
        if (_rightHandModel != null) _rightHandModel.LerpFactor = RightHandSmoothFactor;
    }
}

}