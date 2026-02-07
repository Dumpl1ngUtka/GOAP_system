using System.Collections.Generic;
using Config;
using Items;
using Spells.Configs;
using Units.UnitClasses;
using UnityEngine;
using Zenject;

namespace Services.GameCard
{
    public class GameCardService
    {
        private readonly GameConfig _gameConfig;
        private readonly DiContainer _container;

        public GameCardService(GameConfig gameConfig, DiContainer container)
        {
            _gameConfig = gameConfig;
            _container = container;
        }

        public CardPresenter GetRandomWeaponCard()
        {
            List<WeaponVariantConfig> weapons = _gameConfig.ItemsConfig.Weapons;
            var config = GetRandomElement(weapons);
            return CreateCard<ItemCardPresenter>(config);
        }

        public CardPresenter GetRandomArmorCard()
        {
            List<ArmorVariantConfig> armors = _gameConfig.ItemsConfig.Armors;
            var config = GetRandomElement(armors);
            return CreateCard<ItemCardPresenter>(config);
        }

        public CardPresenter GetRandomSpellCard()
        {
            List<SpellVariantConfig> spells = _gameConfig.SpellsConfig.Spells;
            var config = GetRandomElement(spells);
            return CreateCard<SpellCardPresenter>(config);
        }

        public CardPresenter GetRandomUnitCard()
        {
            List<UnitClass> classes = _gameConfig.ClassesConfig.Classes;
            var config = GetRandomElement(classes);
            return CreateCard<UnitCardPresenter>(config);
        }

        private TCard CreateCard<TCard>(object config) where TCard : CardPresenter
        {
            return config == null ? null : _container.Instantiate<TCard>(new[] { config });
        }

        private T GetRandomElement<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return default;
            }

            int index = Random.Range(0, list.Count);
            return list[index];
        }
    }
}