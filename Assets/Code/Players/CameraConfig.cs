using System;
using UnityEngine;

namespace Code.Players
{
    [Serializable]
    public class CameraConfig
    {
        [field: SerializeField] public bool EnableEdgePan { get; private set; } = true;
        [field: SerializeField] public float MousePanSpeed { get; private set; } = 5f;
        [field: SerializeField] public float EdgePanSize { get; private set; } = 50f; // 픽셀
        [field: SerializeField] public float KeyboardPanSpeed { get; private set; } = 5f;
        
        [field: SerializeField] public float ZoomSpeed { get; private set; } = 1f; // 아직 안씀
        [field: SerializeField] public float RotationSpeed { get; private set; } = 1f;
        [field: SerializeField] public float MinZoomDistance { get; private set; } = 7.5f;
    }
}