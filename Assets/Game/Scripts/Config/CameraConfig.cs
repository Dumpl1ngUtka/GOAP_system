using UnityEngine;

namespace Config
{
    [CreateAssetMenu(
        fileName = "CameraConfig",
        menuName = "Configs/CameraConfig"
    )]
    public class CameraConfig : ScriptableObject
    {
        [Header("Movement Settings")]
        [field: SerializeField] public float MoveSpeed { get; private set; } = 20f;
        [field: SerializeField] public float SmoothTime { get; private set; } = 0.1f;

        [Header("Zoom Settings")]
        [field: SerializeField] public float ZoomSpeed { get; private set; } = 10f;
        [field: SerializeField] public float MinZoom { get; private set; } = 5f;
        [field: SerializeField] public float MaxZoom { get; private set; } = 30f;

        [Header("Observer Mode Settings")]
        [field: SerializeField] public float ObserverRotationSpeed { get; private set; } = 10f;
    }
}