using System.Collections.Generic;
using AI.Knowledge;
using Player.Cards;
using UnityEngine;

namespace Items
{
    public abstract class ItemVariantConfig : ScriptableObject, IObjectForAI, ICard
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public GameObject Model { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        public abstract CardType Type { get; }

        [SerializeField] private string[] _tags;

        public IEnumerable<string> GetTags() => _tags;
    }
}