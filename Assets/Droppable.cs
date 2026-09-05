using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Droppable : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler, IDropHandler
{
    public UnityEvent<Draggable> OnMoveEvent;
    public UnityEvent<Draggable> OnExitEvent;
    public UnityEvent<Draggable> OnDropEvent;

    public void OnPointerMove(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
            if (draggable != null)
            {
                OnMoveEvent?.Invoke(draggable);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
            if (draggable != null)
            {
                OnExitEvent?.Invoke(draggable);
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
            if (draggable != null)
            {
                draggable.Drop();
                OnDropEvent?.Invoke(draggable);
            }
        }
    }
}