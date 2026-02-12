using Units.Config;
using Units.UnitClasses;

namespace Units
{
    public class Parameters
    {
        public ushort Vitality;
        public ushort Dexterity;
        public ushort Strength;
        public ushort Intelligence;

        public Parameters(UnitClassVariantConfig config)
        {
            Vitality = config.Vitality;
            Dexterity = config.Dexterity;
            Strength = config.Strength;
            Intelligence = config.Intelligence;
        }
    }
}