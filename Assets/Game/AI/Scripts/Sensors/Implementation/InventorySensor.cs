using System;
using System.Collections.Generic;
using AI.Knowledge;
using Items;
using Units;

namespace AI.Sensors
{
    public class InventorySensor : ISensor
    {
        public event Action Changed;

        private List<Fact> _cachedFacts = new();
        private float _timeSinceLastUpdate;
        
        private readonly UnitInventory _inventory;

        public InventorySensor(UnitInventory inventory)
        {
            _inventory = inventory;
        }

        public void Start()
        {
            _inventory.Changed += UpdateFacts;
        }

        public void Update(float deltaTime)
        {
        }

        public void Stop()
        {
            _inventory.Changed -= UpdateFacts;
        }

        public IEnumerable<Fact> GetFacts()
        {
            return _cachedFacts;
        }

        private void UpdateFacts()
        {
            List<Fact> newFacts = new();

            if (_inventory.Armor != null)
            {
                AddFactsFromItem(newFacts, _inventory.Armor);
            }

            if (_inventory.ActiveWeapon != null)
            {
                AddFactsFromItem(newFacts, _inventory.ActiveWeapon);
            }

            foreach (var weapon in _inventory.InactiveWeapons)
            {
                if (weapon != null)
                {
                    AddFactsFromItem(newFacts, weapon);
                }
            }

            if (!AreFactsEqual(_cachedFacts, newFacts))
            {
                _cachedFacts = newFacts;
                Changed?.Invoke();
            }
        }

        private void AddFactsFromItem(List<Fact> facts, ItemVariantConfig item)
        {
            foreach (var tag in item.GetTags())
            {
                facts.Add(new Fact(GlobalKeys.ConditionTag.Has, tag));
            }
        }

        private bool AreFactsEqual(List<Fact> list1, List<Fact> list2)
        {
            if (list1.Count != list2.Count) return false;

            var list2Copy = new List<Fact>(list2);

            foreach (var fact1 in list1)
            {
                bool found = false;
                for (int i = 0; i < list2Copy.Count; i++)
                {
                    if (FactsMatch(fact1, list2Copy[i]))
                    {
                        list2Copy.RemoveAt(i);
                        found = true;
                        break;
                    }
                }
                if (!found) return false;
            }

            return true;
        }

        private bool FactsMatch(Fact f1, Fact f2)
        {
            if (f1.ConditionTag != f2.ConditionTag) return false;
            if (f1.ObjectTags.Length != f2.ObjectTags.Length) return false;
            
            for (int i = 0; i < f1.ObjectTags.Length; i++)
            {
                if (f1.ObjectTags[i] != f2.ObjectTags[i]) return false;
            }
            
            return true;
        }
    }
}