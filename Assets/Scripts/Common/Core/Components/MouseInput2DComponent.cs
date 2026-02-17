using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Scripts.Common.Core.Components
{
    public class MouseInput2DComponent : IMouseInput2DComponent
    {
        const int LeftButton = 0;
        const int RightButton = 1;
        const int MiddleButton = 2;

        public event Action<int, Vector2> DragStarted;
        public event Action<int, Vector2> DragDelta;
        public event Action<int> DragEnded;
        public event Action<float> ScrollDelta;

        readonly Vector2[] _lastMousePos = new Vector2[3];
        readonly bool[] _isDragging = new bool[3];
        float _scrollThreshold = 0.01f;

        public void Tick()
        {
            var mousePos = GetMousePosition();

            HandleDrag(LeftButton, mousePos);
            HandleDrag(RightButton, mousePos);
            HandleDrag(MiddleButton, mousePos);

            var scroll = GetMouseScrollDeltaY();
            if (Mathf.Abs(scroll) > _scrollThreshold)
                ScrollDelta?.Invoke(scroll);
        }

        void HandleDrag(int button, Vector2 mousePos)
        {
            if (GetMouseButtonDown(button))
            {
                _isDragging[button] = true;
                _lastMousePos[button] = mousePos;
                DragStarted?.Invoke(button, mousePos);
            }

            if (GetMouseButton(button) && _isDragging[button])
            {
                var delta = mousePos - _lastMousePos[button];
                _lastMousePos[button] = mousePos;
                DragDelta?.Invoke(button, delta);
            }

            if (GetMouseButtonUp(button))
            {
                _isDragging[button] = false;
                DragEnded?.Invoke(button);
            }
        }

        Vector2 GetMousePosition()
        {
#if ENABLE_INPUT_SYSTEM
            var mouse = Mouse.current;
            return mouse != null ? mouse.position.ReadValue() : Vector2.zero;
#else
            return Input.mousePosition;
#endif
        }

        bool GetMouseButtonDown(int button)
        {
#if ENABLE_INPUT_SYSTEM
            return GetMouseButtonState(button, isDown: true, isUp: false);
#else
            return Input.GetMouseButtonDown(button);
#endif
        }

        bool GetMouseButton(int button)
        {
#if ENABLE_INPUT_SYSTEM
            return GetMouseButtonState(button, isDown: false, isUp: false);
#else
            return Input.GetMouseButton(button);
#endif
        }

        bool GetMouseButtonUp(int button)
        {
#if ENABLE_INPUT_SYSTEM
            return GetMouseButtonState(button, isDown: false, isUp: true);
#else
            return Input.GetMouseButtonUp(button);
#endif
        }

        float GetMouseScrollDeltaY()
        {
#if ENABLE_INPUT_SYSTEM
            var mouse = Mouse.current;
            return mouse != null ? mouse.scroll.ReadValue().y : 0f;
#else
            return Input.mouseScrollDelta.y;
#endif
        }

#if ENABLE_INPUT_SYSTEM
        static bool GetMouseButtonState(int button, bool isDown, bool isUp)
        {
            var mouse = Mouse.current;
            if (mouse == null)
                return false;

            var control = button switch
            {
                0 => mouse.leftButton,
                1 => mouse.rightButton,
                2 => mouse.middleButton,
                _ => null
            };

            if (control == null)
                return false;

            if (isDown)
                return control.wasPressedThisFrame;
            if (isUp)
                return control.wasReleasedThisFrame;

            return control.isPressed;
        }
#endif
    }
}
