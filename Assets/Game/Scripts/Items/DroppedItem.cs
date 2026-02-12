using System.Collections.Generic;
using System.Linq;
using AI.Knowledge;
using UnityEngine;
using Zenject;

namespace Items
{
    public class DroppedItem : MonoBehaviour, IWorldObjectForAI, IDroppable
    {
        [SerializeField] private Transform _modelContainer;
        
        private List<string> _tagsForFact;
        
        [Inject]
        public void Construct(ItemVariantConfig itemVariantConfig)
        {
            _tagsForFact = itemVariantConfig.GetTags().ToList();
            _tagsForFact.Add(GlobalKeys.WorldObject.DroppedItem);
        }

        public IEnumerable<string> GetTags()
            => _tagsForFact;

        public Transform GetWorldTransform() 
            => transform;

        private void SpawnMesh(GameObject mesh)
        {
            GameObject instantiate = Instantiate(mesh, _modelContainer);
            instantiate.transform.localPosition = Vector3.zero;
            instantiate.transform.localRotation = Quaternion.identity;
        }

        public void Drop(Vector3 position = default, Quaternion rotation = default)
        {
            Debug.Log("Dropped");
            transform.position = position;
            transform.rotation = rotation;
            gameObject.SetActive(true);
        }

        public void Take()
        {
            Debug.Log("Taking");
            gameObject.SetActive(false);
        }
    }
}