using UnityEngine;

namespace BattleScene
{
    public interface ICardItem
    {
        public int ID { get; }
        public Sprite Sprite { get; }
    }
}