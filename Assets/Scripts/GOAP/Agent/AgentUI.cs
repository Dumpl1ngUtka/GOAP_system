using UI.InGameUI.Bar;
using UnityEngine;

namespace GOAP.Agent
{
    public class AgentUI : MonoBehaviour
    {
        [SerializeField] private Bar _healthBar;

        public void HealthChanged(ushort a, ushort b)
        {
            _healthBar.UpdateValue(a, b);
        }
    }
}