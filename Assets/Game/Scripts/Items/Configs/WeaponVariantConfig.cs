using Player.Cards;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "New weapon", menuName = "Items/Weapon")]
    public class WeaponVariantConfig : ItemVariantConfig
    {
        public override CardType Type => CardType.Weapon;
    }
}