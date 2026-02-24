using System;
using Items;
using Services.Spawner;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units
{
    public class UnitInventory
    {
        public event Action Changed;
        
        public ArmorVariantConfig Armor { get; private set; }
        public WeaponVariantConfig ActiveWeapon { get; private set; }
        
        private readonly WeaponVariantConfig[] _inactiveWeapons = new WeaponVariantConfig[2];
        public WeaponVariantConfig[] InactiveWeapons => _inactiveWeapons;

        private readonly SpawnService _spawnService;
        private readonly Transform _unitTransform;

        public UnitInventory(SpawnService spawnService, Transform unitTransform)
        {
            _spawnService = spawnService;
            _unitTransform = unitTransform;
        }

        public bool AddItem(ItemVariantConfig item)
        {
            if (item is ArmorVariantConfig armor)
            {
                if (Armor == null)
                {
                    Armor = armor;
                    Changed?.Invoke();
                    return true;
                }
            }
            else if (item is WeaponVariantConfig weapon)
            {
                if (ActiveWeapon == null)
                {
                    ActiveWeapon = weapon;
                    Changed?.Invoke();
                    return true;
                }

                for (int i = 0; i < _inactiveWeapons.Length; i++)
                {
                    if (_inactiveWeapons[i] == null)
                    {
                        _inactiveWeapons[i] = weapon;
                        Changed?.Invoke();
                        return true;
                    }
                }
            }
            return false;
        }

        public void DropItem(ItemVariantConfig item)
        {
            if (item == null) return;

            bool removed = false;

            if (Armor == item)
            {
                Armor = null;
                removed = true;
            }
            else if (ActiveWeapon == item)
            {
                ActiveWeapon = null;
                removed = true;
            }
            else
            {
                for (int i = 0; i < _inactiveWeapons.Length; i++)
                {
                    if (_inactiveWeapons[i] == item)
                    {
                        _inactiveWeapons[i] = null;
                        removed = true;
                        break;
                    }
                }
            }

            if (removed)
            {
                Vector3 spawnPosition = _unitTransform.position + Random.insideUnitSphere * 1.5f;
                spawnPosition.y = _unitTransform.position.y;
                _spawnService.SpawnItem(item, spawnPosition, Quaternion.identity);
            }
            Changed?.Invoke();
        }

        public void EquipWeapon(WeaponVariantConfig weapon)
        {
            if (weapon == null || ActiveWeapon == weapon) return;

            for (int i = 0; i < _inactiveWeapons.Length; i++)
            {
                if (_inactiveWeapons[i] == weapon)
                {
                    var previousActive = ActiveWeapon;
                    ActiveWeapon = weapon;
                    _inactiveWeapons[i] = previousActive;
                    Changed?.Invoke();
                    return;
                }
            }
        }
    }
}