using System;
using UnityEngine;

namespace Services.SaveLoad.Data
{
    [Serializable]
    public struct SettingsData
    {
        public bool MusicMute;
        public bool SoundMute;
        public float MusicVolume;
        public float SoundVolume;

        public SettingsData DeepClone() => 
            JsonUtility.FromJson<SettingsData>(JsonUtility.ToJson(this));
    }
}