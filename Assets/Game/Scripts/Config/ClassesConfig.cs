using System.Collections.Generic;
using Units.UnitClasses;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "ClassesConfig", menuName = "Configs/ClassesConfig")]
    public class ClassesConfig : ScriptableObject
    {
        [field: SerializeField] public List<UnitClass> Classes { get; }
    }
}