using System;
using UnityEngine;

namespace Services.GameCard
{
    public abstract class CardPresenter 
    {
        public event Action Changed;
        
        public abstract string Name { get; }
        public abstract Sprite Icon { get; }
        public abstract Sprite TypeIcon { get; }
        
        public abstract void PutOnField(Vector3 position);
        public abstract void PutOnCard(CardPresenter target);

        protected void InvokeChanged()
        {
            Changed?.Invoke();
        }
    }
}