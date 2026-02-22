using UnityEngine;
using UnityEngine.Serialization;

namespace ProceduralAnimations
{
    public class ActiveRagdollLimb : MonoBehaviour
    {
        public ConfigurableJoint _joint;
        public  Transform _targetBone; 
        public  Quaternion _startingRotation;

        public void Setup(ConfigurableJoint joint, Transform targetBone)
        {
            _joint = joint;
            _targetBone = targetBone;
        }
        
        private void Start()
        {
            _startingRotation = transform.localRotation;
        }

        private void FixedUpdate()
        {
            _joint.targetRotation = Quaternion.Inverse(_targetBone.localRotation) * _startingRotation;
        }
    }
}