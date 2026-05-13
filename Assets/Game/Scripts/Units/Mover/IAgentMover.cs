using System;
using UnityEngine;

namespace Units.Mover
{
    public interface IAgentMover
    {
        event Action OnStuck;
        
        Transform GetSelfTransform();
        void SetTargetPosition(Vector3 targetPosition);
        
        void SetTargetRotation(Quaternion targetRotation);
        
        void SetTarget(Vector3 targetPosition, Quaternion targetRotation);
        
        void Stop();
        
        bool IsMoving { get; }
        
        bool IsRotating { get; }
        
        Vector3 CurrentTargetPosition { get; }
        
        Quaternion CurrentTargetRotation { get; }
    }
}