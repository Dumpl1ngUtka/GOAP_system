using System;
using System.Linq;
using Player.Cards;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "Card Base Config", menuName = "Configs/CardBaseConfig")]
    public class CardBaseConfig : ScriptableObject
    {
        [SerializeField] private CardTypeWithIcon[] _typeIcons;
        [field: SerializeField] public int MaxCardsInHand { get; private set; } = 5;
        
        [Header("UI Colors")]
        [field: SerializeField] public Color NormalTextColor { get; private set; } = Color.white;
        [field: SerializeField] public Color LimitReachedTextColor { get; private set; } = Color.red;

        public Sprite GetIconByType(CardType type) => (
                from typeWithIcon in _typeIcons 
                where type == typeWithIcon.Type 
                select typeWithIcon.Icon).FirstOrDefault();
    }


    [Serializable]
    public struct CardTypeWithIcon
    {
        public CardType Type;
        public Sprite Icon;
    }
}