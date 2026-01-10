using UI.Presenters.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View.Windows
{
    public class SpawnWindow : WindowBase
    {
        [SerializeField] private Button _greenSpawnButton;
        [SerializeField] private Button _redSpawnButton;
        
        private ISpawnWindowPresenter _presenter;
        
        
    }
}