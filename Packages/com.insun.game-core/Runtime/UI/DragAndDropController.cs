using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InSun.GameCore.UI
{
    public sealed class DragAndDropController : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas;

        private Action<DropArgs> onDropCallback;

        public GameObject DraggedObject { get; private set; }

        public GameObject DropTarget { get; private set; }

        private void Awake()
        {
            if (canvas == false)
            {
                canvas = GetComponentInParent<Canvas>();
            }
        }

        public void BeginDrag(DragAndDropSource source, PointerEventData eventData, Action<DropArgs> onDrop)
        {
            if (DraggedObject)
            {
                Debug.LogWarning($"Already dragging {DraggedObject.name}", this);
                return;
            }

            onDropCallback = onDrop;

            if (source.IsCreateCopy)
            {
                DraggedObject = source.CreateCopy(canvas.transform);
            }
            else
            {
                DraggedObject = source.gameObject;
                DraggedObject.transform.SetParent(canvas.transform);
            }

            var canvasGroup = DraggedObject.GetComponentInParent<CanvasGroup>();
            if (canvasGroup)
            {
                canvasGroup.blocksRaycasts = false;
            }

            var eventHook = DraggedObject.GetComponentInParent<DragEventHook>();
            if (eventHook)
            {
                eventHook.IsDragging = true;
            }

            var targetSize = source.Transform.sizeDelta;
            var draggedTr = (RectTransform)DraggedObject.transform;
            draggedTr.sizeDelta = targetSize;

            Drag(eventData);

            var draggedTransform = DraggedObject.transform;
            draggedTransform.SetAsLastSibling();
        }

        public void Drag(PointerEventData eventData)
        {
            if (DraggedObject == false)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas.transform,
                eventData.position,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out var localPoint
            );

            DraggedObject.transform.localPosition = localPoint;
            DropTarget = eventData.pointerCurrentRaycast.gameObject;
        }

        public void EndDrag(PointerEventData eventData)
        {
            if (DraggedObject == false)
            {
                Debug.LogWarning("Not dragging any objects", this);
                DropTarget = null;
                return;
            }

            var canvasGroup = DraggedObject.GetComponentInParent<CanvasGroup>();
            if (canvasGroup)
            {
                canvasGroup.blocksRaycasts = true;
            }

            var eventHook = DraggedObject.GetComponentInParent<DragEventHook>();
            if (eventHook)
            {
                eventHook.IsDragging = false;
            }

            onDropCallback?.Invoke(new DropArgs(eventData, DraggedObject));
            onDropCallback = null;

            Destroy(DraggedObject);
            DraggedObject = null;
            DropTarget = null;
        }

        public readonly struct DropArgs
        {
            public PointerEventData PointerEventData { get; }

            public GameObject DroppedObject { get; }

            public DropArgs(PointerEventData pointerEventData, GameObject droppedObject)
            {
                PointerEventData = pointerEventData;
                DroppedObject = droppedObject;
            }
        }
    }
}
