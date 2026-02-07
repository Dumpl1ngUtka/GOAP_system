using UnityEngine;

namespace Spells.Configs
{
    [CreateAssetMenu(fileName = "New spell", menuName = "Spells/Spell")]
    public class SpellVariantConfig : ScriptableObject
    {
        [field: SerializeField] public string SpellName { get; }
    }
}