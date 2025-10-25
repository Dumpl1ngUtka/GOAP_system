using UnityEngine;

namespace Items
{
    public interface IDroppable
    {
        void Drop(Vector3 position = default, Quaternion rotation = default);
        void Take();
    }
}