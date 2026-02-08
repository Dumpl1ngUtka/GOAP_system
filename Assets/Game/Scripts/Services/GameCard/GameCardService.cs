using System;
using System.Collections.Generic;
using Config;
using Items;
using Player.Cards;
using Spells.Configs;
using Units.UnitClasses;
using Zenject;
using Random = UnityEngine.Random;

namespace Services.GameCard
{
    public class GameCardService
    {
        private readonly GameConfig _gameConfig;
        private readonly IInstantiator _instantiator;

        private readonly List<WeaponVariantConfig> _weapons;
        private readonly List<ArmorVariantConfig> _armors;
        private readonly List<SpellVariantConfig> _spells;
        private readonly List<UnitClassVariantConfig> _classes;

        public GameCardService(GameConfig gameConfig, IInstantiator instantiator)
        {
            _gameConfig = gameConfig;
            _instantiator = instantiator;

            _weapons = _gameConfig.ItemsConfig.Weapons;
            _armors = _gameConfig.ItemsConfig.Armors;
            _spells = _gameConfig.SpellsConfig.Spells;
            _classes = _gameConfig.ClassesConfig.Classes;
        }
        
        public CardPresenter GetRandomCardByType(CardType currentCardType)
        {
            return currentCardType switch
            {
                CardType.Duck => GetRandomUnitCard(),
                CardType.Weapon => GetRandomWeaponCard(),
                CardType.Armor => GetRandomArmorCard(),
                CardType.Spell => GetRandomSpellCard(),
                _ => throw new ArgumentOutOfRangeException(nameof(currentCardType), currentCardType, null)
            };
        }

        private CardPresenter GetRandomWeaponCard()
        {
            WeaponVariantConfig config = GetRandomElement(_weapons);
            return CreateCard<WeaponCardPresenter>(config);
        }

        private CardPresenter GetRandomArmorCard()
        {
            ArmorVariantConfig config = GetRandomElement(_armors);
            return CreateCard<ArmorCardPresenter>(config);
        }

        private CardPresenter GetRandomSpellCard()
        {
            SpellVariantConfig config = GetRandomElement(_spells);
            return CreateCard<SpellCardPresenter>(config);
        }

        private CardPresenter GetRandomUnitCard()
        {
            UnitClassVariantConfig config = GetRandomElement(_classes);
            return CreateCard<UnitCardPresenter>(config);
        }

        private TCard CreateCard<TCard>(object config) where TCard : CardPresenter
        {
            return config == null ? null : _instantiator.Instantiate<TCard>(new[] { config });
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