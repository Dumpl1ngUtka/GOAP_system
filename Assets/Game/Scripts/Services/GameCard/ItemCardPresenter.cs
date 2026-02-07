using Items;
using UnityEngine;

namespace Services.GameCard
{
    public class ItemCardPresenter : CardPresenter
    {
        private readonly ItemVariantConfig _config;

        public ItemCardPresenter(ItemVariantConfig config)
        {
            _config = config;
        }

        public override void PutOnField(Vector3 position)
        {
            Debug.Log($"Item {_config.Name} put on field");
        }

        public override void PutOnCard(CardPresenter target)
        {
            Debug.Log($"Item {_config.Name} put on card {target}");
        }
    }
}