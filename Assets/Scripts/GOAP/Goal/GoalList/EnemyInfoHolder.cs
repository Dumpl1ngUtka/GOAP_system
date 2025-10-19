using System.Collections.Generic;
using TEST;
using UnityEngine;

namespace GOAP.Goal.GoalList
{
    public class EnemyInfoHolder
    {
        private List<Enemy> _agents = new List<Enemy>();
        
        public void AddEnemy(Enemy agent) => _agents.Add(agent);

        public List<Enemy> GetAllEnemies() => _agents;

        //TODO change
        public Enemy GetNearestEnemy()
        {
            return _agents.Count == 0 ? null : _agents[Random.Range(0, _agents.Count)];
        }

        public void Clear() => _agents.Clear();
        
        public int GetEnemiesCount() => _agents.Count;
    }
}