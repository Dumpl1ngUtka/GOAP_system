using UnityEngine;

namespace AI.Knowledge
{
    public interface IWorldObjectForAI : IObjectForAI
    {
        Transform GetWorldTransform();
    }
}