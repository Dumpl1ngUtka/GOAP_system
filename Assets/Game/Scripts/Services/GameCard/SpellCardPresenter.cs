using Spells.Configs;
using UnityEngine;

namespace Services.GameCard
{
    public class SpellCardPresenter : CardPresenter
    {
        private readonly SpellVariantConfig _config;

        public SpellCardPresenter(SpellVariantConfig config)
        {
            _config = config;
        }

        public override void PutOnField(Vector3 position)
        {
            Debug.Log($"Spell {_config.SpellName} put on field");
        }

        public override void PutOnCard(CardPresenter target)
        {
            Debug.Log($"Spell {_config.SpellName} put on card {target}");
        }
    }
}