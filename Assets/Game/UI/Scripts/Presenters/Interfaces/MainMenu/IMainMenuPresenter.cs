namespace UI.Presenters.Interfaces.MainMenu
{
    public interface IMainMenuPresenter : IPresenter
    {
        void StartGame();
        void OpenSettings();
        void ExitGame();
    }
}