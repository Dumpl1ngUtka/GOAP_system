using System;
using System.Collections.Generic;
using GOAP.KnowledgeBase;
using OwnSystems.DamageSystem;
using UI.InGameUI.Bar;
using UnityEngine;

namespace TEST
{
    public class Enemy : MonoBehaviour, IWorldObjectForFact, IDamageable
    {
        [SerializeField] private float _health = 100f;
        [SerializeField] private Bar _bar;
        
        private float _currentHealth;
        
        public IEnumerable<ObjectForFactTag> GetTags()
        {
            return new[] { ObjectForFactTag.Enemy };
        }

        public Transform GetWorldTransform() => transform;

        private void OnEnable()
        {
            _bar.UpdateValue(1);
        }

        public void ApplyDamage(float damage)
        {
            _currentHealth -= damage;
            _bar.UpdateValue(_currentHealth, _health);
            if (_currentHealth <= 0f)
                Destroy(gameObject);
        }
    }
}
