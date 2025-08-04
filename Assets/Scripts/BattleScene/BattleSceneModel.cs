using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleScene
{
    public class BattleSceneModel : MonoBehaviour
    {
        private readonly List<PlayerData> _players = new List<PlayerData>();

        public void AddPlayer(PlayerController playerController)
        {
            var playerId = _players.Count;
            var playerData = new PlayerData(playerId);
            playerData.DataChanged += playerController.DataChanged;
            _players.Add(playerData);
            playerController.Init(this, playerId);
        }

        public void AddCardToPlayer(int playerID, ICardItem card) => _players[playerID].AddCard(card);

        public ICardItem GetPlayerCard(int playerID, int cardId) => _players[playerID].GetCardById(cardId);
        
        public int[] GetPlayerCardsID(int playerIndex) => _players[playerIndex].GetCardsID();
        
        public void UseCard(int playerId, int cardId, Vector3 position)
        {
            
        }

    }

    public class PlayerData
    {
        private Dictionary<int, ICardItem> _cards;
        private int _highestIndex = 0;
        
        public int ID {get;private set;}
        public Action DataChanged;

        public PlayerData(int playerId)
        {
            _cards = new Dictionary<int, ICardItem>();
            _highestIndex = 0;
            ID = playerId;
        }
        
        public void AddCards(ICardItem[] cards)
        {
            foreach (var card in cards) 
                _cards.Add(_highestIndex++, card);

            DataChanged?.Invoke();
        }
        
        public void AddCard(ICardItem card)
        {
            _cards.Add(_highestIndex++, card);
            DataChanged?.Invoke();
        }

        public void RemoveCardByID(int id)
        {
            _cards.Remove(id);
            DataChanged?.Invoke();
        }
        
        public ICardItem GetCardById(int id)
        {
            return _cards[id];
        }

        public int[] GetCardsID() => _cards.Keys.ToArray();
    }
}