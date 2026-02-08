using UnityEngine;

namespace Config
{
    [CreateAssetMenu(
        fileName = "AudioConfig",
        menuName = "Configs/AudioConfig"
    )]
    public class AudioConfig : ScriptableObject
    {
        [Header("Music")]
        [field: SerializeField] public AudioClip MainMenuBackgroundMusic{ get; private set; }
        [field: SerializeField] public AudioClip BattleBackgroundMusic{ get; private set; }
        
        [Header("Sound")]
        [field: SerializeField] public AudioClip DefaultButtonSound { get; private set; }
        [field: SerializeField] public AudioClip OpenPopupSound { get; private set; }
        [field: SerializeField] public AudioClip BuyButtonSound { get; private set; }
        [field: SerializeField] public AudioClip ErrorSound { get; private set; }
    }
}