using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace InSun.GameCore.UI
{
    public sealed class DragAndDropSource : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("General")]
        [SerializeField]
        private DragAndDropController dragAndDropController;

        [Header("Dragging")]
        [SerializeField]
        private bool isCreateCopy = true;

// Sirenix.OdinInspector.ShowIf

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isCreateCopy))]
#else
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(isCreateCopy))]
#endif

        [SerializeField]
        private GameObject dragCopyHandle;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onDragEntered;

        [SerializeField]
        private UnityEvent onDragExited;

        [SerializeField]
        private UnityEvent onDropSuccesful;

        public event Action onDropFail;

        private DragAndDropZone hoveredDropZone;

        public RectTransform Transform => (RectTransform)transform;

        public bool IsCreateCopy
        {
            get => isCreateCopy;
            set => isCreateCopy = value;
        }

        public event Action<CopiedArgs> OnCopied;

        private void Awake()
        {
            if (dragAndDropController == false)
            {
                dragAndDropController = GetComponentInParent<DragAndDropController>();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragAndDropController.BeginDrag(this, eventData, OnDrop);
            onDragEntered.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            var dropTarget = eventData.pointerCurrentRaycast.gameObject;
            if (dropTarget == false)
            {
                if (hoveredDropZone)
                {
                    // Were over a drop zome in a previous drag and exited
                    hoveredDropZone.TryExit();
                    hoveredDropZone = null;
                }

                return;
            }

            // TODO: add some check so we don't query parent on each drag tick
            var dropZone = dropTarget.GetComponentInParent<DragAndDropZone>();
            if (dropZone)
            {
                if (dropZone != hoveredDropZone)
                {
                    if (hoveredDropZone)
                    {
                        // Drop zone changed, cleanup current one
                        hoveredDropZone.TryExit();
                        hoveredDropZone = null;
                    }

                    if (dropZone.TryEnter(draggedObject: dragAndDropController.DraggedObject, dropTarget: dropTarget))
                    {
                        // New drop zone entered
                        hoveredDropZone = dropZone;
                    }
                }

                dropZone.Drag(dropTarget, eventData.position);
            }
            else if (hoveredDropZone)
            {
                // Drop zone exited, cleanup
                hoveredDropZone.TryExit();
                hoveredDropZone = null;
            }

            dragAndDropController.Drag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragAndDropController.EndDrag(eventData);

            if (hoveredDropZone)
            {
                // Drag ended and we were over a drop zone, cleanup
                hoveredDropZone.TryExit();
                hoveredDropZone = null;
            }

            onDragExited.Invoke();
        }

        public GameObject CreateCopy(Transform parent)
        {
            var instance = Instantiate(dragCopyHandle ? dragCopyHandle : gameObject, parent);
            instance.SetActive(true);

            OnCopied?.Invoke(new CopiedArgs(instance));

            return instance;
        }

        private void OnDrop(DragAndDropController.DropArgs args)
        {
            var dropTarget = dragAndDropController.DropTarget;
            if (dropTarget == false)
            {
                onDropFail.Invoke();
                return;
            }

            var dropZone = dropTarget.GetComponentInParent<DragAndDropZone>();
            if (dropZone) {
                if (dropZone.TryDrop(dropTarget: dropTarget)) {
                    onDropSuccesful.Invoke();
                } else {
                    onDropFail.Invoke();
                }
            } else {
                onDropFail.Invoke();
            }
        }

        public readonly struct CopiedArgs
        {
            public GameObject CopiedObject { get; }

            public CopiedArgs(GameObject copiedObject)
            {
                CopiedObject = copiedObject;
            }
        }
    }
}
