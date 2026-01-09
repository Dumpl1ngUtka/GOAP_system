using System.Collections.Generic;

namespace Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        T Add(T item);
        void Update(T item);
        void Remove(T item);
    }
}