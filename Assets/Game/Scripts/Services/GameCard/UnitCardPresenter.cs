using Config;
using Units.UnitClasses;
using UnityEngine;

namespace Services.GameCard
{
    public class UnitCardPresenter : CardPresenter
    {
        private readonly UnitClassVariantConfig _config;
        private readonly CardBaseConfig _cardBaseConfig;

        public UnitCardPresenter(
            GameConfig gameConfig,
            UnitClassVariantConfig config)
        {
            _cardBaseConfig = gameConfig.CardBaseConfig;
            _config = config;
        }

        public override string Name => _config.Name;
        public override Sprite Icon => _config.Sprite;
        public override Sprite TypeIcon => _cardBaseConfig.GetIconByType(_config.Type);

        public override void PutOnField(Vector3 position)
        {
            Debug.Log($"Unit {_config.Name} put on field");
        }

        public override void PutOnCard(CardPresenter target)
        {
            Debug.Log($"Unit {_config.Name} put on card {target}");
        }
    }
}