using System.Collections.Generic;
using UnityEngine;

namespace AI.Knowledge
{
    public interface IObjectForAI
    {
        IEnumerable<string> GetTags();
    }
    
    public interface IWorldObjectForAI : IObjectForAI
    {
        Transform GetWorldTransform();
    }
}