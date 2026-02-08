using System;
using Config;
using Items;
using Services.Audio;
using Services.GameCard;
using Services.GameControl;
using Services.GameStates;
using Services.GameStates.StateMachine;
using Services.SaveLoad.Data;
using Services.SaveLoad.Interfaces;
using Services.SaveLoad.Repositories;
using Services.SaveLoad.Services;
using Services.SceneLoader;
using Services.Spawner;
using Units;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [Header("Spawner")]
        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private DroppedItem _itemPrefab;
        [Header("Configs")]
        [SerializeField] private GameConfig _gameConfig;

        public void Awake()
        {
            Container.Resolve<GameStateService>().SetBootstrapState();
        }

        public override void InstallBindings()
        {
            BindAudio();
            BindSpawnService();
            BindSaveLoad();
            BindGameCardService();
            BindGameControlService();
            BindGameStateService();
            BindSceneLoader();
            BindGameConfig();
        }

        private void BindSceneLoader()
        {
            Container
                .Bind<SceneLoader>()
                .AsSingle();
        }

        private void BindSpawnService()
        {
            ObjectSpawner<Unit> unitSpawner = new();
            ObjectSpawner<DroppedItem> itemsSpawner = new();
            
            Container
                .Bind<SpawnService>()
                .AsSingle()
                .WithArguments(unitSpawner, itemsSpawner, _itemPrefab, _unitPrefab);      
        }
        
        private void BindSaveLoad()
        {
            BindJsonSaveLoad();
        }
        
        private void BindJsonSaveLoad()
        {
            Container
                .Bind<ISaveLoadRepository<PlayerData>>()
                .To<JsonSaveLoadRepository<PlayerData>>()
                .AsSingle()
                .WithArguments("playerSave.json");
            
            Container
                .Bind<ISaveLoadRepository<SettingsData>>()
                .To<JsonSaveLoadRepository<SettingsData>>()
                .AsSingle()
                .WithArguments("settingsSave.json");

            Container
                .Bind<SaveLoadService>()
                .AsSingle();
        }
        
        private void BindAudio()
        {
            Container
                .Bind<AudioService>()
                .AsSingle();
        }

        private void BindGameCardService()
        {
            Container
                .Bind<GameCardService>()
                .AsSingle();
        }

        private void BindGameControlService()
        {
            Container
                .Bind<GameControlService>()
                .AsSingle();
        }

        private void BindGameStateService()
        {
            Container
                .Bind<GameStateMachine>()
                .AsSingle();

            Container
                .Bind<GameStateService>()
                .AsSingle();
        }
        
        private void BindGameConfig()
        {
            Container
                .Bind<GameConfig>()
                .FromInstance(_gameConfig)
                .AsSingle();
        }
    }
}