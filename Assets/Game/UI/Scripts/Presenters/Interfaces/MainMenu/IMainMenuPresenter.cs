namespace UI.Presenters.Interfaces.MainMenu
{
    public interface IMainMenuPresenter : IPresenter
    {
        void OpenSettings();
        void StartGame();
        void OpenShop();
        void OpenStatistics();
    }
}