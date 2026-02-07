using Units.UnitClasses;
using UnityEngine;

namespace Services.GameCard
{
    public class UnitCardPresenter : CardPresenter
    {
        private readonly UnitClass _config;

        public UnitCardPresenter(UnitClass config)
        {
            _config = config;
        }

        public override void PutOnField(Vector3 position)
        {
            Debug.Log($"Unit {_config.name} put on field");
        }

        public override void PutOnCard(CardPresenter target)
        {
            Debug.Log($"Unit {_config.name} put on card {target}");
        }
    }
}