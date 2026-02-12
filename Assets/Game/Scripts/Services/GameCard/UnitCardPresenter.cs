using Config;
using Services.SaveLoad.Services;
using Services.Spawner;
using Units.UnitClasses;
using UnityEngine;

namespace Services.GameCard
{
    public class UnitCardPresenter : CardPresenter
    {
        private readonly UnitClassVariantConfig _config;
        private readonly CardBaseConfig _cardBaseConfig;
        private readonly SaveLoadService _saveLoadService;
        private readonly SpawnService _spawnService;

        public UnitCardPresenter(
            GameConfig gameConfig,
            UnitClassVariantConfig config,
            SaveLoadService saveLoadService,
            SpawnService spawnService)
        {
            _cardBaseConfig = gameConfig.CardBaseConfig;
            _config = config;
            _saveLoadService = saveLoadService;
            _spawnService = spawnService;
        }

        public override string Name => _config.Name;
        public override Sprite Icon => _config.Sprite;
        public override Sprite TypeIcon => _cardBaseConfig.GetIconByType(_config.Type);

        public override void PutOnField(Vector3 position)
        {
            _spawnService.SpawnAgent(_config, position, Quaternion.identity);
            Debug.Log($"Unit {_config.Name} spawned at {position}");
        }

        public override void PutOnCard(CardPresenter target)
        {
            Debug.Log($"Unit {_config.Name} put on card {target}");
        }
    }
}