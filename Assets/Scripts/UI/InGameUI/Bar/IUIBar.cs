namespace UI.InGameUI.Bar
{
    public interface IUIBar
    {
        void UpdateValue(float currentValue, float maxValue);
        
        void UpdateValue(float currentValue);
    }
}