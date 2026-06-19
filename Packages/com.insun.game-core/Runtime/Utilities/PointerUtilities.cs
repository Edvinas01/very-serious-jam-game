using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace InSun.GameCore.Utilities
{
    public static class PointerUtilities
    {
        private static readonly List<RaycastResult> RaycastBuffer = new();

        private static PointerEventData currentPointerEvent;
        private static EventSystem pointerEventSystem;
        private static Camera currentCamera;

        public static Vector2 PointerWorldPosition => GetPointerWorldPosition(PointerScreenPosition);

        public static Vector2 PointerScreenPosition
        {
            get
            {
                var pointer = Pointer.current;
                if (pointer == null)
                {
                    return default;
                }

                return pointer.position.ReadValue();
            }
        }

        public static bool IsPointerOverUI
        {
            get
            {
                var pointer = Pointer.current;
                if (pointer == null)
                {
                    return false;
                }

                var eventSystem = EventSystem.current;
                if (eventSystem == false)
                {
                    return false;
                }

                var pointerEvent = GetPointerEvent(eventSystem);
                pointerEvent.position = pointer.position.ReadValue();

                RaycastBuffer.Clear();
                eventSystem.RaycastAll(pointerEvent, RaycastBuffer);

                return RaycastBuffer.Count > 0;
            }
        }

        public static Vector2 GetPointerWorldPosition(Vector2 screenPosition, Camera camera = null)
        {
            if (IsCameraValid(camera))
            {
                // ReSharper disable once PossibleNullReferenceException
                return camera.ScreenToWorldPoint(screenPosition);
            }

            if (TryGetActiveCamera(out var activeCamera))
            {
                return activeCamera.ScreenToWorldPoint(screenPosition);
            }

            return default;
        }

        private static bool TryGetActiveCamera(out Camera camera)
        {
            if (IsCameraValid(currentCamera) == false)
            {
                currentCamera = Camera.main;
            }

            if (IsCameraValid(currentCamera) == false)
            {
                currentCamera = Camera.current;
            }

            if (IsCameraValid(currentCamera) == false)
            {
                camera = null;
                return false;
            }

            camera = currentCamera;
            return true;
        }

        private static bool IsCameraValid(Camera camera)
        {
            if (camera == false)
            {
                return false;
            }

            return camera.isActiveAndEnabled;
        }

        private static PointerEventData GetPointerEvent(EventSystem eventSystem)
        {
            if (currentPointerEvent == null || pointerEventSystem != eventSystem)
            {
                currentPointerEvent = new PointerEventData(eventSystem);
                pointerEventSystem = eventSystem;
            }

            currentPointerEvent.Reset();
            return currentPointerEvent;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void InitializeEditor()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                RaycastBuffer.Clear();
                currentPointerEvent = null;
                pointerEventSystem = null;
                currentCamera = null;
            }
        }
#endif
    }
}
