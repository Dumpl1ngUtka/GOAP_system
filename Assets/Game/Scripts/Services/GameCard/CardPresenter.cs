using UnityEngine;

namespace Services.GameCard
{
    public abstract class CardPresenter
    {
        public abstract void PutOnField(Vector3 position);
        public abstract void PutOnCard(CardPresenter target);
    }
}