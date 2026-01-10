using System;

namespace Units
{
    public interface IHealth
    {
        event Action Changed;
        event Action Died;
        int CurrentHealth { get; }
        int MaxHealth { get; }
        void ApplyDamage(ushort damage);
        void ApplyHealing(ushort healing);
    }
}