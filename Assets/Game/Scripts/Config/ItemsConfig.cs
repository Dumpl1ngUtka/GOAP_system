using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "ItemsConfig", menuName = "Configs/ItemsConfig")]
    public class ItemsConfig : ScriptableObject
    {
        [field: SerializeField] public List<WeaponVariantConfig> Weapons { get; }
        [field: SerializeField] public List<ArmorVariantConfig> Armors { get; }
    }
}