using System;
using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.Popups
{
    /// <summary>
    /// A popup window which requires to enter a text input.
    /// </summary>
    public abstract class ConfirmPopup<TPopup> : EditorWindow where TPopup : ConfirmPopup<TPopup>
    {
        /// <summary>
        /// Title of this popup.
        /// </summary>
        protected abstract string Title { get; }

        /// <summary>
        /// <c>true</c> if user-entered values are valid and the popup can be confirmed.
        /// </summary>
        protected abstract bool IsValid { get; }

        /// <summary>
        /// Width of this popup.
        /// </summary>
        protected virtual float Width => 400f;

        /// <summary>
        /// Height of this popup.
        /// </summary>
        protected virtual float Height => 250f;

        /// <summary>
        /// Called when popup is confirmed.
        /// </summary>
        public event Action OnConfirmed;

        /// <summary>
        /// Called when popup is confirmed.
        /// </summary>
        public event Action OnCanceled;

        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// Show a new popup.
        /// </summary>
        /// <returns>
        /// Popup which was just shown.
        /// </returns>
        public static TPopup Create(bool isSetMousePosition = true)
        {
            var popup = CreateInstance<TPopup>();
            var popupRect = popup.position;

            if (isSetMousePosition)
            {
                var mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                popupRect.x = mousePosition.x;
                popupRect.y = mousePosition.y;
            }

            popupRect.width = popup.Width;
            popupRect.height = popup.Height;

            var minSize = popup.minSize;
            minSize.x = popup.Width;
            minSize.y = popup.Height;

            popup.position = popupRect;
            popup.minSize = minSize;
            popup.titleContent = new GUIContent(popup.Title);

            return popup;
        }

        /// <summary>
        /// Show this popup.
        /// </summary>
        public new void Show()
        {
            ShowModalUtility();
            Focus();

            try
            {
                // Workaround for Unity issue "EndLayoutGroup: BeginLayoutGroup must be called first".
                GUIUtility.ExitGUI();
            }
            catch (ExitGUIException)
            {
                // IDK how to handle this one, just suppress
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private void OnGUI()
        {
            OnDrawPopup();

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = IsValid;
            if (GUILayout.Button("Confirm", GUILayout.Height(30f)))
            {
                OnConfirmed?.Invoke();
                Close();
            }

            GUI.enabled = true;

            if (GUILayout.Button("Cancel", GUILayout.Height(30f)))
            {
                OnCanceled?.Invoke();
                Close();
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Called when popup is drawn.
        /// </summary>
        protected virtual void OnDrawPopup()
        {
        }
    }
}
