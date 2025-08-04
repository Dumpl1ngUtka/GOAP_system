using BattleScene;
using GOAP;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Configs/Item")]
    public class Item : ScriptableObject, IActionHolder, ICardItem
    {
        [Header("Base Item Settings")]
        [SerializeField] private int _itemID;
        [SerializeField] private string _name;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private GameObject _model;
        [Header("Actions")]
        [SerializeField] private Action[] _actions;
        
        public int ID => 1 * 1000 + _itemID;
        public string Name => _name;
        public GameObject Model => _model;
        public Sprite Sprite => _sprite;
        public Action[] GetAction() => _actions;
    }
}