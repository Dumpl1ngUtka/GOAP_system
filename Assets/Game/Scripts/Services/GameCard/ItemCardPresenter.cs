using Config;
using Items;
using UnityEngine;

namespace Services.GameCard
{
    public abstract class ItemCardPresenter : CardPresenter
    {
        public override string Name => Config.Name;
        public override Sprite Icon => Config.Sprite;
        public override Sprite TypeIcon => CardBaseConfig.GetIconByType(Config.Type);
        
        protected readonly ItemVariantConfig Config;
        protected readonly CardBaseConfig CardBaseConfig;

        public ItemCardPresenter(
            GameConfig gameConfig,
            ItemVariantConfig config)
        {
            CardBaseConfig = gameConfig.CardBaseConfig;
            Config = config;
        }

        public abstract override void PutOnField(Vector3 position);

        public abstract override void PutOnCard(CardPresenter target);
    }
}