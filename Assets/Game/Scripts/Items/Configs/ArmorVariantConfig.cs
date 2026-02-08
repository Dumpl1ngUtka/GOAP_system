using Player.Cards;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "New armor", menuName = "Items/Armor")]
    public class ArmorVariantConfig : ItemVariantConfig
    {
        public override CardType Type => CardType.Armor;
    }
}