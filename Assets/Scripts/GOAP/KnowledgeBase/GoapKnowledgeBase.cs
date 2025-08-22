using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.KnowledgeBase
{
    public class GoapKnowledgeBase : IGoapKnowledge
    {
        private readonly Dictionary<string, object> _worldStateFacts = new();
        private readonly Dictionary<string, GameObject> _trackedObjects = new();

        public void SetFact(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new System.ArgumentException("Key cannot be null or empty.", nameof(key));

            _worldStateFacts[key] = value;
        }

        public T GetFact<T>(string key)
        {
            if (TryGetFact<T>(key, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException($"Fact with key '{key}' and type {typeof(T)} not found in knowledge base.");
        }

        public bool TryGetFact<T>(string key, out T value)
        {
            if (_worldStateFacts.TryGetValue(key, out var storedValue))
            {
                if (storedValue is T typedValue)
                {
                    value = typedValue;
                    return true;
                }

                throw new System.InvalidCastException(
                    $"Fact with key '{key}' is of type {storedValue.GetType()}, not {typeof(T)}.");
            }

            value = default;
            return false;
        }

        public bool ContainsFact(string key)
        {
            return _worldStateFacts.ContainsKey(key);
        }

        public void RemoveFact(string key)
        {
            _worldStateFacts.Remove(key);
        }

        public void SetObject(string key, GameObject value)
        {
            if (string.IsNullOrEmpty(key))
                throw new System.ArgumentException("Key cannot be null or empty.", nameof(key));

            _trackedObjects[key] = value;
        }

        public GameObject GetObject(string key)
        {
            if (_trackedObjects.TryGetValue(key, out GameObject value))
            {
                return value;
            }

            throw new KeyNotFoundException($"Object with key '{key}' not found in knowledge base.");
        }

        public bool TryGetObject(string key, out GameObject value)
        {
            return _trackedObjects.TryGetValue(key, out value);
        }

        public IReadOnlyDictionary<string, object> GetAllFacts()
        {
            return new Dictionary<string, object>(_worldStateFacts);
        }

        public IReadOnlyDictionary<string, GameObject> GetAllObjects()
        {
            return new Dictionary<string, GameObject>(_trackedObjects);
        }

        public void ClearTrackedObjects()
        {
            _trackedObjects.Clear();
        }

        public void UpdateFacts(IEnumerable<KeyValuePair<string, object>> newFacts)
        {
            foreach (var fact in newFacts)
            {
                SetFact(fact.Key, fact.Value);
            }
        }
    }
}