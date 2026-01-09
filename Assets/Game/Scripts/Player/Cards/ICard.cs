using UnityEngine;

namespace Player.Cards
{
    public interface ICard
    {
        int ID { get; }
        Sprite Sprite { get; }
        string Title { get; }
        CardType Type { get; }
        string Description { get; }
    }

    public enum CardType
    {
        None = 0,  
        Duck,
        Monster,
        Spell,
    }
}