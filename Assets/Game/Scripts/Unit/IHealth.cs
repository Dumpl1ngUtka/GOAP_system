using System;

namespace Unit
{
    public interface IHealth
    {
        ushort CurrentHealth { get; }
        ushort MaxHealth { get; }
        event Action Changed;
        event Action Died;
        void ApplyDamage(ushort damage);
        void ApplyHealing(ushort healing);
    }
}