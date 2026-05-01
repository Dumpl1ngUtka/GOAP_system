// --- START OF FILE AIGlobalRegistry.cs ---

using System;
using AI.Knowledge;

namespace AI.GlobalRegistry
{
    public static class AIGlobalRegistry
    {
        // События для руды
        public static event Action<IObjectForAI> OnOreSpawned;
        public static event Action<IObjectForAI> OnOreDepleted;

        // События для зданий (передаем объект и команду, которой он принадлежит)
        public static event Action<IObjectForAI, string> OnStructureSpawned;
        public static event Action<IObjectForAI, string> OnStructureAttacked;
        public static event Action<IObjectForAI, string> OnStructureDestroyed;

        public static void ReportOreSpawned(IObjectForAI ore) => OnOreSpawned?.Invoke(ore);
        public static void ReportOreDepleted(IObjectForAI ore) => OnOreDepleted?.Invoke(ore);
        public static void ReportStructureSpawned(IObjectForAI structure, string team) => OnStructureSpawned?.Invoke(structure, team);
        public static void ReportStructureAttacked(IObjectForAI structure, string team) => OnStructureAttacked?.Invoke(structure, team);
        public static void ReportStructureDestroyed(IObjectForAI structure, string team) => OnStructureDestroyed?.Invoke(structure, team);
    }
}