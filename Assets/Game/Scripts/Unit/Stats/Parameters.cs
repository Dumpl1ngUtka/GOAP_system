using System;
using Unit.Config;

namespace Unit
{
    public class Parameters
    {
        public ushort Vitality;
        public ushort Dexterity;
        public ushort Strength;
        public ushort Intelligence;

        public Parameters(ParametersConfig config)
        {
            Vitality = config.Vitality;
            Dexterity = config.Dexterity;
            Strength = config.Strength;
            Intelligence = config.Intelligence;
        }
    }
}