using System;
using Unit.Config;

namespace Unit
{
    public class AgentHealth : IHealth
    {
        public event Action Changed;
        public event Action Died;
        
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; }

        public AgentHealth(
            UnitBaseConfig unitConfig,
            Parameters parameters)
        {
            MaxHealth = unitConfig.HealthPerVitality * parameters.Vitality;
            CurrentHealth = MaxHealth;
        }

        public void ApplyDamage(ushort damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Died?.Invoke();
            }
            Changed?.Invoke();
        }

        public void ApplyHealing(ushort healing)
        {
            CurrentHealth += healing;
            if (CurrentHealth > MaxHealth) 
                CurrentHealth = MaxHealth;
            Changed?.Invoke();
        }
    }
}