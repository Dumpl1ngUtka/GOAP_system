using UnityEngine;

namespace BattleScene
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Card _cardPrefab;
        [SerializeField] private Transform _cardsContainer;
        
        public void Init()
        {
            
        }

        public void RenderCards(ICardItem card)
        {
            
        }

        private void ClearContainer()
        {
            foreach (Transform child in _cardsContainer) 
                Destroy(child.gameObject);
        }
    }

    public class Card : MonoBehaviour
    {
        
    }
}