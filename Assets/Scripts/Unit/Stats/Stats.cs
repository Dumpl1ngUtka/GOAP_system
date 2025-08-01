using System;

namespace Unit.Stats
{
    public class Stats
    {
        public float MaxHealth { get; private set; }
        public float CurrentHealth {get; private set;}
        
        public Action IsDead { get; private set; }
        public Action<float, float> HealthChanged { get; private set; }

        public Stats(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = MaxHealth;
        }

        public void ApplyDamage(float damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                IsDead?.Invoke();
                return;
            }
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void ChangeMaxHealth(float newValue)
        {
            MaxHealth = newValue;
            HealthChanged?.Invoke(MaxHealth, MaxHealth);
        }
    }
}