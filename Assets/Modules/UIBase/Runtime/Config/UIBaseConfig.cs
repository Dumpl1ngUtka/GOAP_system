using UnityEngine;

[CreateAssetMenu(menuName = "Modules/UIBase/Create UIBaseConfig", fileName = "UIBaseConfig")]
public sealed class UIBaseConfig : ScriptableObject
{
    [field: SerializeField] public float DurationEndFightMessage { get; private set; }
    [field: SerializeField] public float DelayEndFightPopup { get; private set; }
}
