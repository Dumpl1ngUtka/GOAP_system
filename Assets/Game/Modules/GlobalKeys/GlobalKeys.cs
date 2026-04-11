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
        public const string DoNothing = nameof(DoNothing);
        public const string IsPatrolling = nameof(IsPatrolling);
    }
    
    public class Resources
    {
        public const string Health = nameof(Health);
        public const string Mana = nameof(Mana);
        public const string Wood = nameof(Wood);
        public const string Gold = nameof(Gold);
    }
    
    public class Tool
    {
        public const string Woodcutter = nameof(Woodcutter);
        public const string Pickaxe = nameof(Pickaxe);
        public const string MeleeWeapon = nameof(MeleeWeapon);
    }

    public class WorldObject
    {
        public const string Enemy = nameof(Enemy);
        public const string Ally = nameof(Ally);
        public const string DroppedItem = nameof(DroppedItem);
        public const string Tree = nameof(Tree);
        public const string GoldOre = nameof(GoldOre);
    }
    
    public class UI
    {
        public class Popup
        {
            public const string SettingsPopup = nameof(SettingsPopup);
            public const string PausePopup = nameof(PausePopup);
        }

        public class Widget
        {
            public const string SellZoneWidget = nameof(SellZoneWidget);
        }
        
        public class Window
        {
            public const string HUD = nameof(HUD);
            public const string MainMenuWindow = nameof(MainMenuWindow);
        }
    }

    public class Team
    {
        public const string Alpha = nameof(Alpha);
        public const string Beta = nameof(Beta);
    }

    public class Scene
    {
        public const string GameScene = nameof(GameScene);
        public const string MainMenuScene = nameof(MainMenuScene);
        public const string BootstrapScene = nameof(BootstrapScene);
    }
}