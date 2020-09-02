using UnityEngine;

[CreateAssetMenu(fileName = "CubeConfig", menuName = "CubeConfig", order = 1)]
public class CubeConfig : ScriptableObject
{
    public float DelayNextCube = 2f;
    public float SpeedBall;

    public float UpSpeedPlatform = 0.3f;
    public float SwapSpeedPlatform = 0.3f;

    public RotateContainer Rotate;
    public ZoomContainer Zoom;
    public AudioContainer Audio;



    [System.Serializable]
    public class RotateContainer
    {
        public float SpeedRotate;
        public float MinDistance = 5f;
    }

    [System.Serializable]
    public class ZoomContainer
    {
        public float MouseZoomSpeed = 15.0f;
        public float TouchZoomSpeed = 0.1f;
        public float ZoomMinBound = 40f;
        public float ZoomMaxBound = 60f;
    }

    [System.Serializable]
    public class AudioContainer
    {
        public AudioClip BackgroundMusic;
        public AudioClip ClickPlatform;
        public AudioClip Finish;

    }
}

