using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] CanvasGroup canvasGroup;

    float hoverScale = 1.1f;

    Vector3 originalLocalScale;
    Vector3 dragOffset;

    bool isDragging = false;

    public Action<Draggable> OnBeginDragEvent;

    private void Start()
    {
        originalLocalScale = transform.localScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragEvent?.Invoke(this);
        canvasGroup.blocksRaycasts = false;
        BeginMove(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Move(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndMove();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        BeginMove(eventData);
        Move(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EndMove();
    }

    void BeginMove(PointerEventData eventData)
    {
        if (isDragging)
        {
            return;
        }
        isDragging = true;

        dragOffset = transform.position - (Vector3)eventData.position;

        transform.DOScale(originalLocalScale * hoverScale, 0.15f);

        transform.SetAsLastSibling();
    }

    void Move(PointerEventData eventData)
    {
        transform.position = (Vector3)eventData.position + dragOffset;
    }

    void EndMove()
    {
        if (!isDragging)
        {
            return;
        }
        isDragging = false;

        //transform.localPosition = originalLocalPos;
        //transform.localScale = originalLocalScale;
        transform.DOScale(originalLocalScale, 0.15f);
        canvasGroup.blocksRaycasts = true;
    }

    public void Drop()
    {
        EndMove();
    }
}