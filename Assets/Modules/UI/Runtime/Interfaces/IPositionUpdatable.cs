using System;

public interface IPositionUpdatable
{
    event Action OnPositionUpdated;
    float DefaultYPosition { get; }
    void UpdateDefaultYPosition(float defaultPosition);
}