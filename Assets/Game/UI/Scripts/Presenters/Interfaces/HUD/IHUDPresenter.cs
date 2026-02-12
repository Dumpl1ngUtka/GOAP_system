using System;
using System.Collections.Generic;
using Services.GameCard;
using UnityEngine;

namespace UI.Presenters.Interfaces.HUD
{
    public interface IHUDPresenter : IPresenter
    {
        void Pause();
        List<CardPresenter> GetCards();
        int GetMaxCardsCount();
        Color CardsCountColor { get; }
        string CardsCountText { get; }
    }
}