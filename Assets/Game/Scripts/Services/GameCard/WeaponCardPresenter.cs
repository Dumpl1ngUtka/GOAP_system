using Config;
using Items;
using UnityEngine;

namespace Services.GameCard
{
    public class WeaponCardPresenter : ItemCardPresenter
    {
        public WeaponCardPresenter(GameConfig gameConfig, WeaponVariantConfig config) : base(gameConfig, config)
        {
        }

        public override void PutOnField(Vector3 position)
        {
            Debug.Log($"Weapon {Config.Name} put on field");
        }

        public override void PutOnCard(CardPresenter target)
        {
            Debug.Log($"Weapon {Config.Name} put on card {target}");
        }
    }
}