using System.Collections.Generic;
using AI.Knowledge;
using UnityEngine;

namespace Items
{
    public abstract class ItemVariantConfig : ScriptableObject, IObjectForAI
    {
        [field: SerializeField] public string Name { get; }
        [field: SerializeField] public GameObject Model { get; }
        [field: SerializeField] public Sprite Sprite { get; }

        [SerializeField] private string[] _tags;

        public IEnumerable<string> GetTags() => _tags;
    }
}