using System;
using UnityEngine;

namespace InSun.GameCore.UI
{
    public sealed class DragAndDropZone : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private RectTransform dropParent;

        private GameObject currentDraggedObject;
        private bool isDragValidPrev;

        public RectTransform DropParent => dropParent;

        public Func<EnterValidatorArgs, bool> EnterValidator { get; set; }

        public Func<DragValidatorArgs, bool> DragValidator { get; set; }

        public Func<DropValidatorArgs, bool> DropValidator { get; set; }

        public event Action<DragEnteredArgs> OnDragEntered;

        public event Action<DragExitedArgs> OnDragExited;

        public event Action<DraggedArgs> OnObjectDragged;

        public event Action<DroppedArgs> OnObjectDropped;

        public bool TryEnter(GameObject draggedObject, GameObject dropTarget)
        {
            if (IsEnterValid(draggedObject) == false)
            {
                return false;
            }

            currentDraggedObject = draggedObject;

            UpdateDragValidity(dropTarget: dropTarget);

            var args = new DragEnteredArgs(draggedObject);
            OnDragEntered?.Invoke(args);

            return true;
        }

        public bool TryExit()
        {
            if (currentDraggedObject == false)
            {
                return false;
            }

            UpdateDragValidity(dropTarget: null);

            var args = new DragExitedArgs(currentDraggedObject);
            OnDragExited?.Invoke(args);

            currentDraggedObject = null;

            return true;
        }

        public void Drag(GameObject dropTarget, Vector2 position)
        {
            if (currentDraggedObject == false)
            {
                return;
            }

            UpdateDragValidity(dropTarget);

            var args = new DraggedArgs(currentDraggedObject, dropTarget, position);
            OnObjectDragged?.Invoke(args);
        }

        public bool TryDrop(GameObject dropTarget)
        {
            if (IsDropValid(dropTarget) == false)
            {
                return false;
            }

            var args = new DroppedArgs(currentDraggedObject, dropTarget);
            OnObjectDropped?.Invoke(args);
            return true;
        }

        private void UpdateDragValidity(GameObject dropTarget)
        {
            var isDragValidNext = IsDragValid(currentDraggedObject, dropTarget);
            if (isDragValidNext != isDragValidPrev)
            {
                var eventHook = currentDraggedObject.GetComponentInParent<DragEventHook>();
                if (eventHook)
                {
                    eventHook.IsValidZone = isDragValidNext;
                }
            }

            isDragValidPrev = isDragValidNext;
        }

        private bool IsEnterValid(GameObject draggedObject)
        {
            if (draggedObject == false)
            {
                return false;
            }

            if (EnterValidator == null)
            {
                return true;
            }

            var args = new EnterValidatorArgs(draggedObject);
            return EnterValidator.Invoke(args);
        }

        private bool IsDropValid(GameObject dropTarget)
        {
            if (currentDraggedObject == false)
            {
                return false;
            }

            if (DropValidator == null)
            {
                return true;
            }

            var args = new DropValidatorArgs(currentDraggedObject, dropTarget);
            return DropValidator.Invoke(args);
        }

        private bool IsDragValid(GameObject draggedObject, GameObject dropTarget)
        {
            if (draggedObject == false)
            {
                return false;
            }

            if (dropTarget == false)
            {
                return false;
            }

            if (DragValidator == null)
            {
                return true;
            }

            var args = new DragValidatorArgs(draggedObject, dropTarget);
            return DragValidator.Invoke(args);
        }

        public readonly struct EnterValidatorArgs
        {
            public GameObject DraggedObject { get; }

            public EnterValidatorArgs(GameObject draggedObject)
            {
                DraggedObject = draggedObject;
            }
        }

        public readonly struct DragValidatorArgs
        {
            public GameObject DraggedObject { get; }

            public GameObject DropTarget { get; }

            public DragValidatorArgs(GameObject draggedObject, GameObject dropTarget)
            {
                DraggedObject = draggedObject;
                DropTarget = dropTarget;
            }
        }

        public readonly struct DropValidatorArgs
        {
            public GameObject DraggedObject { get; }

            public GameObject DropTarget { get; }

            public DropValidatorArgs(GameObject draggedObject, GameObject dropTarget)
            {
                DraggedObject = draggedObject;
                DropTarget = dropTarget;
            }
        }

        public readonly struct DragEnteredArgs
        {
            public GameObject DraggedObject { get; }

            public DragEnteredArgs(GameObject draggedObject)
            {
                DraggedObject = draggedObject;
            }
        }

        public readonly struct DragExitedArgs
        {
            public GameObject DraggedObject { get; }

            public DragExitedArgs(GameObject draggedObject)
            {
                DraggedObject = draggedObject;
            }
        }

        public readonly struct DraggedArgs
        {
            public GameObject DraggedObject { get; }

            public GameObject DropTarget { get; }

            public Vector2 Position { get; }

            public DraggedArgs(GameObject draggedObject, GameObject dropTarget, Vector2 position)
            {
                DraggedObject = draggedObject;
                DropTarget = dropTarget;
                Position = position;
            }
        }

        public readonly struct DroppedArgs
        {
            public GameObject DroppedObject { get; }

            public GameObject DropTarget { get; }

            public DroppedArgs(GameObject droppedObject, GameObject dropTarget)
            {
                DroppedObject = droppedObject;
                DropTarget = dropTarget;
            }
        }
    }
}
