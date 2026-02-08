using UnityEngine;

namespace Player.Cards
{
    public interface ICard
    {
        int ID { get; }
        string Name { get; }
        Sprite Sprite { get; }
        CardType Type { get; }
        string Description { get; }
    }

    public enum CardType
    {
        None = 0,  
        Duck,
        Weapon,
        Armor,
        Spell,
    }
}