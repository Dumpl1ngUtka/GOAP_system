public static class GlobalKeys
{
    public class ConditionTag
    {
        public const string Has = nameof(Has);
        
        public const string Nearby = nameof(Nearby);
        public const string Around = nameof(Around);
        public const string InSight = nameof(InSight);
        
        public const string IsLow = nameof(IsLow);
        public const string IsHigh = nameof(IsHigh);
    }
    
    public class Resources
    {
        public const string Health = nameof(Health);
        public const string Mana = nameof(Mana);
    }
    
    public class Tool
    {
        public const string Woodcutter = nameof(Woodcutter);
        public const string MeleeWeapon = nameof(MeleeWeapon);
    }

    public class WorldObject
    {
        public const string Enemy = nameof(Enemy);
        public const string EnemyTower = nameof(EnemyTower);
        public const string OwnTower = nameof(OwnTower);
    }
    
    public class UI
    {
        public class Popup
        {
            public const string SettingsPopup = nameof(SettingsPopup);
        }

        public class Widget
        {
            public const string SellZoneWidget = nameof(SellZoneWidget);
        }
        
        public class Window
        {
            public const string GameLoopWindow = nameof(GameLoopWindow);
        }
    }
}