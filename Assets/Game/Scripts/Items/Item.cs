using System;
using System.Collections.Generic;
using GOAP;
using GOAP.Action;
using GOAP.KnowledgeBase;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items
{
    public abstract class Item : ScriptableObject, IObjectForFact
    {
        [Header("Base Item Settings")]
        [SerializeField] private string _name;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private GameObject _model;
        [Header("Actions")]
        [SerializeField] private ObjectForFactTag[] _tags;
        public string Name => _name;
        public GameObject Model => _model;
        public Sprite Sprite => _sprite;

        public int Id { get; }
        public IEnumerable<ObjectForFactTag> GetTags() 
            => _tags;
    }
}