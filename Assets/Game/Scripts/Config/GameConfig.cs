using Services.SaveLoad.Data;
using Units.Config;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(
        fileName = "GameConfig",
        menuName = "Configs/GameConfig"
    )]
    public class GameConfig : ScriptableObject
    {
        //[Header("Game Parameters")]
        
        [Header("Save Load Parameters")]
        [field: SerializeField] private PlayerData _startPlayerData;
        [field: SerializeField] private SettingsData _startSettingsData;
        
        [Header("Configs")]
        [field: SerializeField] public AudioConfig AudioConfig { get; private set; }
        [field: SerializeField] public SettingsConfig SettingsConfig { get; private set; }
        [field: SerializeField] public ItemsConfig ItemsConfig { get; private set; }
        [field: SerializeField] public SpellsConfig SpellsConfig { get; private set; }
        [field: SerializeField] public ClassesConfig ClassesConfig { get; private set; }
        [field: SerializeField] public CardBaseConfig CardBaseConfig { get; private set; }
        [field: SerializeField] public UnitBaseConfig UnitBaseConfig { get; private set; }
        [field: SerializeField] public CameraConfig CameraConfig { get; private set; }

        public PlayerData StartPlayerData => _startPlayerData.DeepClone();
        public SettingsData StartSettingsData => _startSettingsData.DeepClone();
    }
}