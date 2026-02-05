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
        [field: SerializeField] public AudioClip DefaultBackgroundMusic{ get; private set; }
        
        [Header("Sound")]
        [field: SerializeField] public AudioClip DefaultButtonSound { get; private set; }
        [field: SerializeField] public AudioClip BuyButtonSound { get; private set; }
        [field: SerializeField] public AudioClip OpenPopupSound { get; private set; }
        [field: SerializeField] public AudioClip JellyInfoCellSound { get; private set; }
        [field: SerializeField] public AudioClip CoinsSound { get; private set; }
        [field: SerializeField] public AudioClip MergeSound { get; private set; }
        [field: SerializeField] public AudioClip SellSound { get; private set; }
        [field: SerializeField] public AudioClip CrystalsSound { get; private set; }
        [field: SerializeField] public AudioClip SkipsSound { get; private set; }
        [field: SerializeField] public AudioClip PrestigeSound { get; private set; }
        [field: SerializeField] public AudioClip ContainerSound { get; private set; }
        [field: SerializeField] public AudioClip ErrorSound { get; private set; }
        
        [Header("Sound Settings")]
        [field: SerializeField, Range(0, 3)] public float JellyPitch { get; private set; }
        [field: SerializeField, Range(0, 1)] public float JellyPitchRandomRange { get; private set; }
    }
}