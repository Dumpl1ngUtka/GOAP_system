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
        private float _targetZoom;
        private Vector3 _currentVelocity;
        private float _zoomVelocity;

        private Vector3 _observerTarget;
        private Vector3 _observerOffset;

        [Inject]
        public void Construct(CameraConfig config, PlayerInput playerInput)
        {
            _config = config;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            _targetPosition = transform.position;

            _targetZoom = _cam.orthographic ? _cam.orthographicSize : _cam.fieldOfView;
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

            Vector3 move = new Vector3(input.x, 0, input.y);
            // Adjust movement to be relative to camera rotation (yaw only)
            move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * move;
            
            _targetPosition += move * (_config.MoveSpeed * Time.deltaTime);
        }

        private void HandleCameraZoom(float scroll)
        {
            if (_currentMode != CameraMode.Free) return;
            if (_config == null) return;

            _targetZoom -= scroll * _config.ZoomSpeed;
            _targetZoom = Mathf.Clamp(_targetZoom, _config.MinZoom, _config.MaxZoom);
        }

        private void HandleObserverBehavior()
        {
            if (_config == null) return;

            // Rotate around the target
            transform.position = _observerTarget + _observerOffset;
            transform.LookAt(_observerTarget);
            
            // Rotate the offset around the target
            _observerOffset = Quaternion.AngleAxis(_config.ObserverRotationSpeed * Time.deltaTime, Vector3.up) * _observerOffset;
        }

        private void MoveCameraFree()
        {
            if (_config == null) return;

            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _currentVelocity, _config.SmoothTime);

            if (_cam.orthographic)
            {
                _cam.orthographicSize = Mathf.SmoothDamp(_cam.orthographicSize, _targetZoom, ref _zoomVelocity, _config.SmoothTime);
            }
            else
            {
                _cam.fieldOfView = Mathf.SmoothDamp(_cam.fieldOfView, _targetZoom, ref _zoomVelocity, _config.SmoothTime);
            }
        }
    }
}