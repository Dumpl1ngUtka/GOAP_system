using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(
        fileName = "SettingsConfig",
        menuName = "Configs/SettingsConfig"
    )]
    public class SettingsConfig : ScriptableObject
    {
        //[Header("Language")]
        //[field: SerializeField] public List<LanguageData> Languages { get; private set; }

        /*public LanguageType GetLanguageTypeByCode(string code)
        {
            foreach (var lang in Languages)
            {
                if (string.Equals(lang.Code, code, StringComparison.OrdinalIgnoreCase))
                {
                    return lang.LanguageType;
                }
            }
            return LanguageType.English; 
        }

        public string GetCodeByType(LanguageType type)
        {
            foreach (LanguageData lang in Languages.Where(lang => lang.LanguageType == type))
            {
                return lang.Code;
            }

            return "en";
        }*/
    }

    [Serializable]
    public class LanguageData
    {
        public LanguageType LanguageType;

        [Tooltip("ISO код языка от Яндекса (ru, en, tr)")]
        public string Code;
        public Sprite LanguageIcon;
    }
    
    public enum LanguageType
    {
        None = 0,
        Russian = 1,
        English = 2,
        Turkish = 3
    }
}