using AI.Agent;
using GOAP.Agent;
using OwnSystems.DamageSystem;
using UnityEngine;

namespace TEST
{
    public class TakeHit : MonoBehaviour
    {
        [SerializeField] private float _damage = 10; 
        [SerializeField] private bool _take;
        [SerializeField] private AIAgent _damageableAgent;
    
        private void OnValidate()
        {
            if (_take)
            {
                _damageableAgent.ApplyDamage(_damage);
                _take = false;
            }
        }
    }
}