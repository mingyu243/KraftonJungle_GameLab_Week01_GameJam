using DG.Tweening;
using System;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    [SerializeField] Card card;

    public Card Card => card;

    public void SetCard(Card newCard)
    {
        card = newCard;

        // 다시 카드가 움직이면 슬롯에서 빼낼 수 있도록
        Draggable draggable = card.GetComponent<Draggable>();
        draggable.OnBeginDragEvent += BeginDragEvent;

        ResetPosition();
    }

    public void ResetPosition()
    {
        if (card != null)
        {
            card.transform.DOMove(this.transform.position, 0.1f);
        }
    }

    public void RemoveCard(bool withCheckExpression = false, bool destroyCardObject = false)
    {
        if (card != null)
        {
            // 이벤트 제거
            Draggable draggable = card.GetComponent<Draggable>();
            draggable.OnBeginDragEvent -= BeginDragEvent;

            if (destroyCardObject)
            {
                Destroy(card.gameObject);
            }

            card = null;

            if (withCheckExpression)
            {
                GameManager.Instance.CheckAnswerCurrentExpression();
            }
        }
    }

    void BeginDragEvent(Draggable draggable)
    {
        RemoveCard(true);
    }
}
