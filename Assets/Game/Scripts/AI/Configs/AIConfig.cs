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
        [field: SerializeField] public int BuyJellyPrice { get; private set; }
    }
}