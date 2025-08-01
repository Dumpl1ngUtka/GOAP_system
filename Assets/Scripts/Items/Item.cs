using GOAP;
using UnityEngine;

namespace Items
{
    public class Item : ScriptableObject, IActionHolder
    {
        [Header("Base Item Settings")]
        [SerializeField] private string _name;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private GameObject _model;
        [Header("Actions")]
        [SerializeField] private Action[] _actions;
        public string Name => _name;
        public GameObject Model => _model;
        public Sprite Sprite => _sprite;
        public Action[] GetAction() => _actions;
    }
}