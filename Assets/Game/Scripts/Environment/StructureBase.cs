using System.Collections.Generic;
using AI.GlobalRegistry;
using AI.Knowledge;
using UnityEngine;

namespace Environment
{
    public class StructureBase : MonoBehaviour, IWorldObjectForAI
    {
        [SerializeField] private string _teamKey = "TeamA";
        [SerializeField] private StructureType _structureType;
        [SerializeField] private float _health = 500f;

        private void Start()
        {
            // Сообщаем ИИ о появлении здания (чтобы Командир знал, что нужно защищать)
            AIGlobalRegistry.ReportStructureSpawned(this, _teamKey);
        }

        // Вызывается, когда по зданию проходит урон
        public void TakeDamage(float damage)
        {
            _health -= damage;
            
            // Кричим Командиру, что нас бьют!
            AIGlobalRegistry.ReportStructureAttacked(this, _teamKey);

            if (_health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            AIGlobalRegistry.ReportStructureDestroyed(this, _teamKey);
            Destroy(gameObject);
        }

        // Реализация IWorldObjectForAI
        public IEnumerable<string> GetTags()
        {
            // Возвращаем теги: Structure, и конкретный тип (FriendlyTower/FriendlyThrone)
            // Примечание: "Friendly/Enemy" будет определять сам Командир в своей базе знаний
            return new[] { GlobalKeys.WorldObject.Structure, _structureType.ToString() }; 
        }

        public Transform GetWorldTransform()
        {
            return transform;
        }
    }
}