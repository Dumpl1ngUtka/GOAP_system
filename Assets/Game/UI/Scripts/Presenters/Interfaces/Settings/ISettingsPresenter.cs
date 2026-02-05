namespace UI.Presenters.Interfaces.Settings
{
    public interface ISettingsPresenter : IPresenter
    {
        void Close();
        float MusicVolume { get; }
        float SoundVolume { get; }
        void SetSoundVolume(float volume);
        void SetMusicVolume(float volume);
        void SetSoundMute(bool unmute);
        void SetMusicMute(bool unmute);
        bool IsMusicMute { get; }
        bool IsSoundMute { get; }
    }
}