using UnityEngine;

namespace Units.Mover
{
    [RequireComponent(typeof(Rigidbody))]
    public class AgentMover : MonoBehaviour, IAgentMover
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _accelerationFactor = 50f;
        [SerializeField] private float _maxVelocity = 10f;
        [SerializeField] private float _stoppingDistance = 1f;
        [SerializeField] private float _rotationSpeed = 180f;
        [SerializeField] private float _angularAccelerationFactor = 50f;
        [SerializeField] private float _maxAngularVelocity = 360f;
        [SerializeField] private float _stoppingAngle = 1f;

        private Rigidbody _rigidbody;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private bool _hasTargetPosition;
        private bool _hasTargetRotation;

        public bool IsMoving => _hasTargetPosition;

        public bool IsRotating =>
            _hasTargetRotation && Quaternion.Angle(transform.rotation, _targetRotation) > _stoppingAngle;

        public Vector3 CurrentTargetPosition => _targetPosition;
        public Quaternion CurrentTargetRotation => _targetRotation;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            if (_rigidbody.isKinematic)
            {
                Debug.LogWarning("Rigidbody on " + gameObject.name +
                                 " is kinematic. AgentMover relies on a non-kinematic Rigidbody for realistic physics interaction. Setting isKinematic to false.");
                _rigidbody.isKinematic = false;
            }
        }

        public Transform GetSelfTransform()
        {
            return transform;
        }

        public void SetTargetPosition(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            _hasTargetPosition = true;
        }

        public void SetTargetRotation(Quaternion targetRotation)
        {
            _targetRotation = targetRotation;
            _hasTargetRotation = true;
        }

        public void SetTarget(Vector3 targetPosition, Quaternion targetRotation)
        {
            SetTargetPosition(targetPosition);
            SetTargetRotation(targetRotation);
        }

        public void Stop()
        {
            _hasTargetPosition = false;
            _hasTargetRotation = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            if (!_hasTargetPosition) return;
    
            // Создаем проекции позиций на плоскость (игнорируем ось Y для расчета дистанции)
            Vector3 currentFlatPos = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 targetFlatPos = new Vector3(_targetPosition.x, 0f, _targetPosition.z);

            // Если достигли точки - сбрасываем флаг и выходим
            if (Vector3.Distance(currentFlatPos, targetFlatPos) <= _stoppingDistance)
            {
                _hasTargetPosition = false;
                _rigidbody.linearVelocity = Vector3.zero; // Гасим остаточную инерцию
                return;
            }

            Vector3 nextPosition = Vector3.MoveTowards(
                transform.position,
                _targetPosition,
                _moveSpeed * Time.fixedDeltaTime
            );

            // Move using Rigidbody
            _rigidbody.MovePosition(nextPosition);
        }

        private void HandleRotation()
        {
            if (!_hasTargetRotation) return;

            Quaternion currentRotation = _rigidbody.rotation;
            float angleToTarget = Quaternion.Angle(currentRotation, _targetRotation);

            if (angleToTarget <= _stoppingAngle)
            {
                _rigidbody.angularVelocity = Vector3.zero;
                _hasTargetRotation = false;
                return;
            }

            float angle;
            Vector3 axis;
            (currentRotation * Quaternion.Inverse(_targetRotation)).ToAngleAxis(out angle, out axis);

            if (angle > 180f) angle -= 360f;

            float desiredAngularSpeedRad = _rotationSpeed * Mathf.Deg2Rad;

            Vector3 desiredAngularVelocity = axis.normalized * (Mathf.Sign(angle) * desiredAngularSpeedRad);
            Vector3 angularVelocityError = desiredAngularVelocity - _rigidbody.angularVelocity;
            Vector3 torque = angularVelocityError * _angularAccelerationFactor;

            _rigidbody.AddTorque(torque, ForceMode.Acceleration);

            float maxAngularVelocityRad = _maxAngularVelocity * Mathf.Deg2Rad;
            if (_rigidbody.angularVelocity.sqrMagnitude > maxAngularVelocityRad * maxAngularVelocityRad)
            {
                _rigidbody.angularVelocity = _rigidbody.angularVelocity.normalized * maxAngularVelocityRad;
            }
        }
    }
}