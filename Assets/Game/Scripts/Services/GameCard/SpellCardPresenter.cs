using Config;
using Spells.Configs;
using UnityEngine;

namespace Services.GameCard
{
    public class SpellCardPresenter : CardPresenter
    {
        private readonly SpellVariantConfig _config;
        private readonly CardBaseConfig _cardBaseConfig;

        public SpellCardPresenter(
            GameConfig gameConfig,
            SpellVariantConfig config)
        {
            _cardBaseConfig = gameConfig.CardBaseConfig;
            _config = config;
        }

        public override string Name => _config.Name;
        public override Sprite Icon => _config.Sprite;
        public override Sprite TypeIcon => _cardBaseConfig.GetIconByType(_config.Type);

        public override void PutOnField(Vector3 position)
        {
            Debug.Log($"Spell {_config.Name} put on field");
        }

        public override void PutOnCard(CardPresenter target)
        {
            Debug.Log($"Spell {_config.Name} put on card {target}");
        }
    }
}