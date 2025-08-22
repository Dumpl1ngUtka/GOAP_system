using UnityEngine;

namespace GOAP.KnowledgeBase
{
    public interface IGoapKnowledge
    {
        void SetFact(string key, object value);
        T GetFact<T>(string key);
        bool TryGetFact<T>(string key, out T value);
        bool ContainsFact(string key);
        void RemoveFact(string key);
    
        void SetObject(string key, GameObject value);
        GameObject GetObject(string key);
        bool TryGetObject(string key, out GameObject value);
    }
}