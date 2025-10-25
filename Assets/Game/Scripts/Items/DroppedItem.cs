using System.Collections.Generic;
using System.Linq;
using GOAP.Action;
using GOAP.KnowledgeBase;
using UnityEngine;

namespace Items
{
    public class DroppedItem : MonoBehaviour, IWorldObjectForFact, IDroppable
    {
        [SerializeField] private Transform _modelContainer;
        private IEnumerable<ObjectForFactTag> _tagsForFact;
        private int _id;
        
        public void Init(Item item)
        {
            _tagsForFact = ((IObjectForFact)item).GetTags();
            _tagsForFact = _tagsForFact.Append(ObjectForFactTag.DroppableItem);
        }

        public int Id { get; }

        public IEnumerable<ObjectForFactTag> GetTags()
            => _tagsForFact;

        public Transform GetWorldTransform() 
            => transform;

        private void SpawnGameObject(GameObject @object)
        {
            var instantiate = Instantiate(@object, _modelContainer);
            instantiate.transform.localPosition = Vector3.zero;
            instantiate.transform.localRotation = Quaternion.identity;
        }

        public void Drop(Vector3 position = default, Quaternion rotation = default)
        {
            Debug.Log("Dropped");
        }

        public void Take()
        {
            Debug.Log("Taking");
        }
    }
}