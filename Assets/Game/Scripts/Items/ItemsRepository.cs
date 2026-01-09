using System.Collections.Generic;
using Interfaces;

namespace Items
{
    public class ItemsRepository : IRepository<Item>
    {
        private List<Item> _items;

        public ItemsRepository()
        {
            _items = new List<Item>();
        }
        
        public IEnumerable<Item> GetAll() => _items;

        public Item GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public Item Add(Item item)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Item item)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(Item item)
        {
            throw new System.NotImplementedException();
        }
    }
}