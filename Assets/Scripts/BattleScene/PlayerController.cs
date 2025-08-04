using UnityEngine;

namespace BattleScene
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerView _view;
        private BattleSceneModel _model;
        private int _id;

        public void Init(BattleSceneModel model, int playerId)
        {
            _model = model;
            _id = playerId;
            //_view.Init();
        }
        
        public void UseCard(int cardId, Vector3 position)
        {
            //_model.UseCard(_id, cardId);
        }

        public void DataChanged()
        {
            var cards = _model.GetPlayerCardsID(_id);
        }
    }
}