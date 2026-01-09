using System;
using System.Collections.Generic;
using Player.Cards;
using UnityEngine;

namespace GameScene.Server
{
    public class Server : IGameServer
    {
        
        
        public Server()
        {
            
        }
        
        public IEnumerable<HandCard> GetStartCards(int count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HandCard> GetCards(int count)
        {
            throw new NotImplementedException();
        }

        public HandCard MergeCards(HandCard firstCard, HandCard secondCard)
        {
            throw new NotImplementedException();
        }

        public void UseCard(HandCard card, Vector3 position)
        {
            throw new NotImplementedException();
        }

        public void UseCard(HandCard card, int objectId)
        {
            throw new NotImplementedException();
        }
    }
}