using System;
using System.Linq;
using Player.Cards;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "Card Base Config", menuName = "Config/CardBaseConfig")]
    public class CardBaseConfig : ScriptableObject
    {
        [SerializeField] private CardTypeWithIcon[] _typeIcons;

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