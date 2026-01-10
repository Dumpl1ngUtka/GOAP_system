using UnityEngine;

namespace Units.Config
{
    [CreateAssetMenu(
        fileName = "ParametersConfig",
        menuName = "Unit/Configs/ParametersConfig"
    )]
    public class UnitConfig : ScriptableObject
    {
        [field: SerializeField] public ushort Vitality { get; private set; }
        [field: SerializeField] public ushort Dexterity { get; private set; }
        [field: SerializeField] public ushort Strength { get; private set; }
        [field: SerializeField] public ushort Intelligence { get; private set; }
    }
}