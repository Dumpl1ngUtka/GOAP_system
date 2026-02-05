using System;
using System.Collections.Generic;
using Config;
using Services.SaveLoad.Data;
using Services.SaveLoad.Interfaces;

namespace Services.SaveLoad.Services
{
    public class SaveLoadService
    {
        public event Action OnPlayerDataChanged;
        public event Action OnSettingsDataChanged;
        
        private readonly ISaveLoadRepository<PlayerData> _playerProgressRepository;
        private readonly ISaveLoadRepository<SettingsData> _settingsRepository;
        
        private PlayerData _sessionData;
        private SettingsData _sessionSettingsData;
        
        private bool _sessionDataLoaded = false;
        private bool _sessionSettingsDataLoaded = false;
        
        private readonly GameConfig _gameConfig;

        public SaveLoadService(
            ISaveLoadRepository<PlayerData> playerProgressRepository,
            ISaveLoadRepository<SettingsData> settingsRepository,
            GameConfig gameConfig)
        {
            _playerProgressRepository = playerProgressRepository;
            _settingsRepository = settingsRepository;
            _gameConfig = gameConfig;   
        }
        
        public void SavePlayerData(PlayerData data)
        {
            data.LastSaveTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            Save(ref _sessionData,  
                ref _sessionDataLoaded, 
                data, 
                _playerProgressRepository, 
                OnPlayerDataChanged);
        }

        public PlayerData LoadPlayerData()
        {
            return Load(ref _sessionData,  
                ref _sessionDataLoaded, 
                _playerProgressRepository, 
                _gameConfig.StartPlayerData);
        }

        public void ResetPlayerData()
        {
            _sessionDataLoaded = false;

            if (_playerProgressRepository.HasSave()) 
                _playerProgressRepository.RemoveSave();
        }
        
        public void QuickSaveSettings(SettingsData data)
        {
            _sessionSettingsData = data;
            OnSettingsDataChanged?.Invoke();
        }

        public void SaveSettings(SettingsData data)
        {
            Save(
                ref _sessionSettingsData,  
                ref _sessionSettingsDataLoaded, 
                data, 
                _settingsRepository,
                OnSettingsDataChanged);
        }

        public SettingsData LoadSettings()
        {
            return Load(ref _sessionSettingsData,
                ref _sessionSettingsDataLoaded,
                _settingsRepository,
                _gameConfig.StartSettingsData);
        }

        private static void Save<T>(
            ref T sessionData, 
            ref bool isLoadedFlag, 
            T currentData, 
            ISaveLoadRepository<T> repository,
            Action changeCallback) 
            where T : struct
        {
            sessionData = currentData;
            isLoadedFlag = true;
            repository.Save(currentData);
            changeCallback?.Invoke();
        }

        private static T Load<T>(
            ref T sessionData, 
            ref bool isLoadedFlag, 
            ISaveLoadRepository<T> repository,
            T newData) 
            where T : struct
        {
            if (isLoadedFlag) 
                return sessionData;

            T returnedData =
                repository.HasSave()
                    ? repository.Load()
                    : newData;
        
            isLoadedFlag = true;
            sessionData = returnedData;
            return returnedData;
        }
    }
}