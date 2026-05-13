using UnityEngine;

namespace AI.Configs
{
    [CreateAssetMenu(
        fileName = "AIConfig",
        menuName = "AI/Configs/AIConfig"
    )]
    public class AIConfig : ScriptableObject
    {
        [Header("Configs")]
        [field: SerializeField] public float NearbyDistance { get; private set; } = 1f;
        [field: SerializeField] public float AroundDistance { get; private set; } = 10f;
        [field: SerializeField] public float InSightDistance { get; private set; } = 30f;

        [Header("Stuck Detection")]
        [field: SerializeField] public float StuckCheckInterval { get; private set; } = 0.5f;
        [field: SerializeField] public float StuckDistanceThreshold { get; private set; } = 0.1f;
        
        public float StuckDistanceThresholdSqr => StuckDistanceThreshold * StuckDistanceThreshold;

        public string[] TeamKeys => new[]
        {
            GlobalKeys.Team.TeamA,
            GlobalKeys.Team.TeamB
        };
    }
}