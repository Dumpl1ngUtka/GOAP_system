using System.Collections.Generic;
using Spells.Configs;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "SpellsConfig", menuName = "Configs/SpellsConfig")]
    public class SpellsConfig : ScriptableObject
    {
        [field: SerializeField] public List<SpellVariantConfig> Spells { get; }
    }
}