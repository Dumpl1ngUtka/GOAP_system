using UnityEngine;

namespace Unit
{
    public class Unit : MonoBehaviour
    {
        public Stats.Stats Stats { get; private set; }  = new Stats.Stats(100);
        public IHealth Health { get; set; }

        public void Init()
        {
            
        }
    }
}