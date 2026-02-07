using System.Collections.Generic;

namespace AI.Knowledge
{
    public interface IObjectForAI
    {
        IEnumerable<string> GetTags();
    }
}