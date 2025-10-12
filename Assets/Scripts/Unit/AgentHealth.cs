using System;

namespace Unit
{
    public class AgentHealth : IHealth
    {
        private ushort _currentHealth;
        private ushort _maxHealth;
        public ushort CurrentHealth => _currentHealth;
        
        public ushort MaxHealth => _maxHealth;
        
        public event Action Changed;
        public event Action Died;

        public AgentHealth(ushort maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = _maxHealth;
        }

        public void ApplyDamage(ushort damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Died?.Invoke();
            }
            Changed?.Invoke();
        }

        public void ApplyHealing(ushort healing)
        {
            _currentHealth += healing;
            if (_currentHealth > MaxHealth) 
                _currentHealth = MaxHealth;
            Changed?.Invoke();
        }
    }
}