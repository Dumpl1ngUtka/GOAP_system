using Player.Cards;
using UnityEngine;

namespace Units.UnitClasses
{
    [CreateAssetMenu(fileName = "New Classes", menuName = "Unit/Class")]
    public class UnitClassVariantConfig : ScriptableObject, ICard
    {
        [field: SerializeField] public int ID { get; }
        [field: SerializeField] public string Name { get; }
        [field: SerializeField] public Sprite Sprite { get; }
        [field: SerializeField] public string Description { get; }
        public CardType Type => CardType.Duck;
    }
}