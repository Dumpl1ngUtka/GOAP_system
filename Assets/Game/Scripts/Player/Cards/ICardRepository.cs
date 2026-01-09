using System.Collections.Generic;
using Interfaces;

namespace Player.Cards
{
    public interface ICardRepository
    {
        IEnumerable<ICard> GetAll();
        IEnumerable<ICard> GetByType(CardType type);
        ICard GetById(int id);
        
    }
}