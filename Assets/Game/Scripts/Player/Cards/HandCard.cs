using JetBrains.Annotations;

namespace Player.Cards
{
    public struct HandCard
    {
        public int ID;
        [CanBeNull] public HandCard[] MergedCards;
        public ICard BaseCard;
    }
}