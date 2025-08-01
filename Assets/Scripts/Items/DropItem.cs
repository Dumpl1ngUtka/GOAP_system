using GOAP;
using UnityEngine;

namespace Items
{
    public class DropItem : MonoBehaviour, IActionHolder
    {
        [SerializeField] private Transform _modelContainer;
        [SerializeField] private TakeItem _takeItemAction;
        private Item _item;

        public void Init(Item item)
        {
            _item = item;
            var model = Instantiate(item.Model, _modelContainer);
        }

        public Action[] GetAction() => new[] { _takeItemAction.GetInstance() };
    }
}