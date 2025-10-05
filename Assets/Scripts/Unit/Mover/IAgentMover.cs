using UnityEngine;

namespace Unit.Mover
{
    public interface IAgentMover
    {
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