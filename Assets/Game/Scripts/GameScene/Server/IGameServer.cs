using System.Collections.Generic;
using Player.Cards;
using UnityEngine;

namespace GameScene.Server
{
    public interface IGameServer
    {
        IEnumerable<HandCard> GetStartCards(int count);
        IEnumerable<HandCard> GetCards(int count);
        HandCard MergeCards(HandCard firstCard, HandCard secondCard);
        void UseCard(HandCard card, Vector3 position);
        void UseCard(HandCard card, int objectId);
    }
}