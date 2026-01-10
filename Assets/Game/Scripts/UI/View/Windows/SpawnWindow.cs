using UI.Presenters.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View.Windows
{
    public class SpawnWindow : MonoBehaviour
    {
        [SerializeField] private Button _greenSpawnButton;
        [SerializeField] private Button _redSpawnButton;
        
        private ISpawnWindowPresenter _presenter;
        
        
    }
}