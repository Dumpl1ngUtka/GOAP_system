using Player.Cards;
using UnityEngine;

namespace Units.UnitClasses
{
    [CreateAssetMenu(fileName = "New Classes", menuName = "Unit/Class")]
    public class UnitClassVariantConfig : ScriptableObject, ICard
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public ushort Vitality { get; private set; }
        [field: SerializeField] public ushort Dexterity { get; private set; }
        [field: SerializeField] public ushort Strength { get; private set; }
        [field: SerializeField] public ushort Intelligence { get; private set; }
        [field: SerializeField] public UnitRole Role{ get; private set; }
        public CardType Type => CardType.Duck;
    }
    
    public enum UnitRole
    {
        Worker,
        MeleeCombat,
        RangedCombat,
        Support
    }
}