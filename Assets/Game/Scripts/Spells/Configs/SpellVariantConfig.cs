using Player.Cards;
using UnityEngine;

namespace Spells.Configs
{
    [CreateAssetMenu(fileName = "New spell", menuName = "Spells/Spell")]
    public class SpellVariantConfig : ScriptableObject, ICard
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        public CardType Type => CardType.Spell;
    }
}