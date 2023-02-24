using UnityEngine;
using System.IO;
using TensorFlowLite;

namespace parahand
{
    public class ParaHand
    {
        public static int JointCount = 21;
        
        public bool IsLeft;
        public float SideScore;
        public float Score;
        public Vector3[] Joints = new Vector3[21];
    }
    public class ParaHandLandmarkPredictor
    {
        private const int InputWidth = 224;
        private const int InputHeight = 224;
        public readonly ParaHand Hand;

        private readonly Interpreter _interpreter;
        private readonly float[,,] _inputTensor = new float[InputWidth, InputHeight, 3];
        private readonly float[] _sideScore = new float[ParaHand.JointCount * 3]; // keypoint
        private readonly float[] _score = new float[1]; // keypoint
        private readonly float[] _normalizedLandmark = new float[1]; // hand flag
        private readonly float[] _worldLandmark = new float[ParaHand.JointCount * 3]; // keypoint

        private readonly RenderTexture _resizeTexture;
        private readonly Texture2D _inputTexture;

        public ParaHandLandmarkPredictor(string modelPath)
        {
            var options = new InterpreterOptions();
            options.AddGpuDelegate();
            
            _interpreter = new Interpreter(FileUtil.LoadFile(modelPath), options);

            Hand = new ParaHand()
            {
                Score = 0,
                Joints = new Vector3[ParaHand.JointCount],
            };
            
            _resizeTexture = new RenderTexture(InputWidth, InputHeight, 0, RenderTextureFormat.ARGB32);
            _inputTexture = new Texture2D(InputWidth, InputHeight, TextureFormat.RGBA32, false);
        }

        public void Invoke(WebCamTexture texture)
        {
            ToTensorCPU(texture, _inputTensor);
            _interpreter.SetInputTensorData(0, _inputTensor);
            _interpreter.Invoke();
            _interpreter.GetOutputTensorData(0, _sideScore);
            _interpreter.GetOutputTensorData(1, _score);
            _interpreter.GetOutputTensorData(2, _normalizedLandmark);
            _interpreter.GetOutputTensorData(3, _worldLandmark);
            
            UpdateHand();
        }

        private void ToTensorCPU(WebCamTexture texture, float[,,] inputs)
        {
            int width = _inputTexture.width;
            int height = _inputTexture.height - 1;

            Graphics.Blit(texture, _resizeTexture);
            var prev = RenderTexture.active;
            RenderTexture.active = _resizeTexture;
            _inputTexture.ReadPixels(new Rect(0, 0, InputWidth, InputHeight), 0, 0);
            _inputTexture.Apply();
            RenderTexture.active = prev;
            
            var pixels = _inputTexture.GetRawTextureData<Color32>();
            const float scale = 255f;
            for (int i = 0; i < pixels.Length; i++)
            {
                int y = height - i / width;
                int x = i % width;
                inputs[y, x, 0] = (pixels[i].r) / scale;
                inputs[y, x, 1] = (pixels[i].g) / scale;
                inputs[y, x, 2] = (pixels[i].b) / scale;
            }
        }

        private void UpdateHand()
        {
            Hand.Score = _score[0];
            Hand.SideScore = _sideScore[0];

            Hand.IsLeft = _sideScore[0] > 100.0;

            for (int i = 0; i < ParaHand.JointCount; i++)
            {
                Hand.Joints[i] = new Vector3(_worldLandmark[i * 3], -_worldLandmark[i * 3 + 1], _worldLandmark[i * 3 + 2]);
            }
        }
        
        public void Dispose()
        {
            _interpreter?.Dispose();
        }
    }
    
    public class ParaHandTrackingManager
    {
        private readonly WebCamTexture _texture;
        private readonly ParaHandLandmarkPredictor _predictor;
        
        public delegate void OnHandDetectedDelegate(ParaHand hand);
        
        public event OnHandDetectedDelegate OnHandDetected;
        public ParaHandTrackingManager()
        {
            var devices = WebCamTexture.devices;
            if (devices.Length <= 0) return;
        
            var device = devices[0];
            _texture = new WebCamTexture(device.name, 224, 224, 30);
            _texture.Play();
            Debug.Log($" camera texture size {_texture.width} x {_texture.height}");

            var modelPath = "Assets/Resources/tflite/hand_landmark_full.tflite";
            _predictor = new ParaHandLandmarkPredictor(modelPath);
        }
        
        public WebCamTexture GetWebCamTexture()
        {
            return _texture;
        }

        public void Predict()
        {
            if (_predictor == null || _texture == null || !_texture.didUpdateThisFrame) return;
            _predictor.Invoke(_texture);
            OnHandDetected?.Invoke(_predictor.Hand);
        }

        public void Dispose()
        {
            _texture.Stop();
            _predictor?.Dispose();
        }

        
        private static ParaHandTrackingManager instance_;
        public static ParaHandTrackingManager Instance {
            get {
                if (instance_ == null) {
                    instance_ = new ParaHandTrackingManager();
                }

                return instance_;
            }
        }

    }
}