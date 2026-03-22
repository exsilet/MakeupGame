using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandDragHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform handRect;
    [SerializeField] private Canvas rootCanvas;

    public event Action OnDragBegin;
    public event Action<Vector2> OnDragMove;
    public event Action<Vector2> OnDragEnd;

    private bool _dragEnabled = false;

    public void EnableDrag()  => _dragEnabled = true;
    public void DisableDrag() => _dragEnabled = false;
    public void OnPointerDown(PointerEventData eventData) { }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_dragEnabled) return;
        OnDragBegin?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_dragEnabled) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.GetComponent<RectTransform>(),
            eventData.position,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
            out Vector2 localPoint
        );

        handRect.anchoredPosition = localPoint;
        OnDragMove?.Invoke(localPoint);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_dragEnabled) return;
        OnDragEnd?.Invoke(eventData.position);
    }
}