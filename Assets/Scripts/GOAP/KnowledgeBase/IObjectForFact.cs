using System.Collections.Generic;
using GOAP.Action;
using UnityEngine;

namespace GOAP.KnowledgeBase
{
    public interface IObjectForFact
    {
        IEnumerable<ObjectForFactTag> GetTags();
    }

    public interface IWorldObjectForFact : IObjectForFact
    {
        Transform GetWorldTransform();
    }

    public enum ObjectForFactTag
    {
        RangeWeapon,
        MeleeWeapon,
        DroppableItem,
        Enemy,
    }
}