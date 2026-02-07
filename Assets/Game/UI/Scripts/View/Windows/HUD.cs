using System;
using System.Collections.Generic;
using UI.Presenters.Interfaces.HUD;
using UI.Presenters.Interfaces.Settings;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.View.Windows
{
    public class HUD : WidgetBase
    {
        [Header("Buttons")]
        [SerializeField] private Button _pauseButton;
        
        private IHUDPresenter _presenter;
        
        public override void Show(Dictionary<string, object> extraData = null, Action endCallback = null)
        {
            base.Show(extraData, endCallback);

            if (PresenterUtils.Setup(extraData,
                    key: Id,
                    subscribeAction: HandleChanged,
                    presenterField: ref _presenter))
            {
                HandleChanged();
                
                _presenter.Changed += HandleChanged; 
                
                _pauseButton.onClick.AddListener(_presenter.Pause);
            }
        }

        public override void Hide(Action endCallback = null)
        {
            PresenterUtils.Teardown(ref _presenter, HandleChanged, startAction: () =>
            {
                _presenter.Changed -= HandleChanged; 
                
                _pauseButton.onClick.RemoveListener(_presenter.Pause);
            });
            
            base.Hide(endCallback);
        }
        
        private void HandleChanged()
        {
        }
    }
}