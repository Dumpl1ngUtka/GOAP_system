using Config;
using Items;
using UnityEngine;

namespace Services.GameCard
{
    public class ArmorCardPresenter : ItemCardPresenter
    {
        public ArmorCardPresenter(GameConfig gameConfig, ArmorVariantConfig config) : base(gameConfig, config)
        {
        }

        public override void PutOnField(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public override void PutOnCard(CardPresenter target)
        {
            throw new System.NotImplementedException();
        }
    }
}