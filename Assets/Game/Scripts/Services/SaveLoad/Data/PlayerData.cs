using System;
using System.Collections.Generic;
using Services.SaveLoad.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Services.SaveLoad.Data
{
    [Serializable]
    public struct PlayerData : ISaveWithTime
    {
        public long LastSaveTime 
        {
            get => lastSaveTime;
            set => lastSaveTime = value;
        }        
        
        public long lastSaveTime; 
        public ulong SoftMoney;

        public PlayerData DeepClone() => 
            JsonUtility.FromJson<PlayerData>(JsonUtility.ToJson(this));
    }
}