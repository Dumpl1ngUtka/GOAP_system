using Config;
using UnityEngine;
using Zenject;

namespace Controllers.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraController : MonoBehaviour
    {
        private enum CameraMode
        {
            Free,
            Observer
        }
        
        private CameraConfig _config;
        private PlayerInput _playerInput;

        private UnityEngine.Camera _cam;
        private CameraMode _currentMode = CameraMode.Free;

        private Vector3 _targetPosition;
        private Vector3 _currentVelocity;

        private Vector3 _observerTarget;
        private Vector3 _observerOffset;

        [Inject]
        public void Construct(
            GameConfig config, 
            PlayerInput playerInput)
        {
            _config = config.CameraConfig;
            _playerInput = playerInput;
            Debug.Log("CameraController constructed");
        }

        private void Awake()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            _targetPosition = transform.position;
        }

        private void Start()
        {
            if (_playerInput != null)
            {
                _playerInput.OnCameraMove += HandleCameraMove;
                _playerInput.OnCameraZoom += HandleCameraZoom;
            }
        }

        private void OnDestroy()
        {
            if (_playerInput != null)
            {
                _playerInput.OnCameraMove -= HandleCameraMove;
                _playerInput.OnCameraZoom -= HandleCameraZoom;
            }
        }

        private void Update()
        {
            if (_currentMode == CameraMode.Observer)
            {
                HandleObserverBehavior();
            }
            else
            {
                MoveCameraFree();
            }
        }

        public void SetFreeMode()
        {
            _currentMode = CameraMode.Free;
            _targetPosition = transform.position;
            _currentVelocity = Vector3.zero;
        }

        public void SetObserverMode(Vector3 target, Vector3 offset)
        {
            _currentMode = CameraMode.Observer;
            _observerTarget = target;
            _observerOffset = offset;
        }

        private void HandleCameraMove(Vector2 input)
        {
            if (_currentMode != CameraMode.Free) return;
            if (_config == null) return;
            
            float zoomPercent = Mathf.InverseLerp(_config.MinZoom, _config.MaxZoom, _targetPosition.y);
            
            float minSpeedMultiplier = 0.3f; 
            float currentSpeed = _config.MoveSpeed * Mathf.Lerp(minSpeedMultiplier, 1f, zoomPercent);
            
            Vector3 move = new Vector3(input.x, 0, input.y);
            move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * move;
    
            _targetPosition += move * (currentSpeed * Time.deltaTime);
        }
        
        private void HandleCameraZoom(float scroll)
        {
            if (_currentMode != CameraMode.Free) return;
            if (_config == null) return;

            Vector3 zoomStep = transform.forward * (scroll * _config.ZoomSpeed);
            Vector3 expectedPosition = _targetPosition + zoomStep;

            if (expectedPosition.y >= _config.MinZoom && expectedPosition.y <= _config.MaxZoom)
            {
                _targetPosition = expectedPosition;
            }
            else
            {
                float targetY = expectedPosition.y < _config.MinZoom ? _config.MinZoom : _config.MaxZoom;
                float differenceY = targetY - _targetPosition.y;
                
                if (Mathf.Abs(transform.forward.y) > 0.001f) 
                {
                    float distanceToLimit = differenceY / transform.forward.y;
                    _targetPosition += transform.forward * distanceToLimit;
                }
            }
        }

        private void HandleObserverBehavior()
        {
            if (_config == null) return;

            transform.position = _observerTarget + _observerOffset;
            transform.LookAt(_observerTarget);
            
            _observerOffset = Quaternion.AngleAxis(_config.ObserverRotationSpeed * Time.deltaTime, Vector3.up) * _observerOffset;
        }

        private void MoveCameraFree()
        {
            if (_config == null) return;

            transform.position = Vector3.SmoothDamp(
                transform.position, 
                _targetPosition, 
                ref _currentVelocity, 
                _config.SmoothTime
            );
        }
    }
}