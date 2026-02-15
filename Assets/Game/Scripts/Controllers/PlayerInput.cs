using System;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class PlayerInput : ITickable
    {
        public event Action<Vector2> OnCameraMove;
        public event Action<float> OnCameraZoom;

        private float dragSensitivity = 1f;

        private Vector3 _dragOrigin;
        private bool _isDragging;

        public void Tick()
        {
            HandleKeyboardInput();
            HandleMouseDragInput();
            HandleZoomInput();
        }

        private void HandleKeyboardInput()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
            {
                // Normalize vector to prevent faster diagonal movement
                Vector2 input = new Vector2(h, v);
                if (input.magnitude > 1f) input.Normalize();
                
                OnCameraMove?.Invoke(input);
            }
        }

        private void HandleMouseDragInput()
        {
            if (Input.GetMouseButtonDown(2)) // Middle mouse button
            {
                _dragOrigin = Input.mousePosition;
                _isDragging = true;
            }

            if (Input.GetMouseButtonUp(2))
            {
                _isDragging = false;
            }

            if (_isDragging)
            {
                Vector3 currentMousePos = Input.mousePosition;
                Vector3 difference = currentMousePos - _dragOrigin;
                
                // Normalize difference based on screen size
                Vector2 moveDelta = new Vector2(difference.x / Screen.width, difference.y / Screen.height);
                
                // Invert drag direction (dragging right moves camera left)
                moveDelta = -moveDelta * dragSensitivity;

                if (moveDelta.sqrMagnitude > 0.0001f)
                {
                    OnCameraMove?.Invoke(moveDelta);
                }

                _dragOrigin = currentMousePos;
            }
        }

        private void HandleZoomInput()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.001f)
            {
                OnCameraZoom?.Invoke(scroll);
            }
        }
    }
}