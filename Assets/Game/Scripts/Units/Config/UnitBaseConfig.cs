using UnityEngine;

namespace Units.Config
{
    [CreateAssetMenu(
        fileName = "UnitBaseConfig",
        menuName = "Configs/UnitBaseConfig"
    )]
    public class UnitBaseConfig : ScriptableObject
    {
        [field: SerializeField] public ushort HealthPerVitality { get; private set; }
    }
}