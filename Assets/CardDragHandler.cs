using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CardSlotManager cardSlotManager;

    List<CardSlot> CardSlotList => cardSlotManager.CardSlotList;

    Dictionary<Card, (int, int)> previewCardSlotIndexDict = new();
    int previewTargetSlotIndex = -1;

    public void OnDraggableExit(Draggable draggable)
    {
        SetBlockRaycastCardSlot(true);
        ResetCardVisualPosition();
    }

    public void OnDraggableMove(Draggable draggable)
    {
        SetBlockRaycastCardSlot(false);

        // 가장 가까운 슬롯 찾기
        int closestSlotIndex = FindClosestSlotIndex(draggable.transform.position);
        CardSlot closestSlot = CardSlotList[closestSlotIndex];

        // 같으면 그냥 놔둠
        if (previewTargetSlotIndex == closestSlotIndex)
        {
            return;
        }

        previewTargetSlotIndex = closestSlotIndex;

        // 좌표 기준으로는 어느 방향으로 밀어야할지
        bool baseIsPushLeft = 0 < (draggable.transform.position.x - closestSlot.transform.position.x);

        // 왼쪽으로 밀 수 있는지
        bool canPushLeft = false;
        if (0 < closestSlotIndex)
        {
            for (int i = (closestSlotIndex - 1); i >= 0; i--)
            {
                CardSlot slot = CardSlotList[i];
                if (slot.Card == null)
                {
                    canPushLeft = true;
                    break;
                }
            }
        }
        // 오른쪽으로 밀 수 있는지
        bool canPushRight = false;
        if (closestSlotIndex < (CardSlotList.Count - 1))
        {
            for (int i = (previewTargetSlotIndex + 1); i < CardSlotList.Count; i++)
            {
                CardSlot slot = CardSlotList[i];
                if (slot.Card == null)
                {
                    canPushRight = true;
                    break;
                }
            }
        }

        // 방향 확정
        bool canPush1 = baseIsPushLeft ? canPushLeft : canPushRight;
        bool canPush2 = baseIsPushLeft ? canPushRight : canPushLeft;
        bool isPushLeft = true;
        if (canPush1)
        {
            isPushLeft = baseIsPushLeft;
        }
        else if (canPush2)
        {
            isPushLeft = !baseIsPushLeft;
        }
        else
        {
            // 양 방향 다 못 밈
            return;
        }

        // 현재 카드의 슬롯 인덱스, 프리뷰로 보여질 카드의 슬롯 인덱스
        previewCardSlotIndexDict.Clear();
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            if (CardSlotList[i].Card != null)
            {
                previewCardSlotIndexDict[CardSlotList[i].Card] = (i, i);
            }
        }

        if (isPushLeft)
        {
            for (int i = previewTargetSlotIndex; i > 0; i--)
            {
                if (CardSlotList[i].Card != null)
                {
                    int originIndex = previewCardSlotIndexDict[CardSlotList[i].Card].Item1;
                    int previewIndex = originIndex - 1;
                    previewCardSlotIndexDict[CardSlotList[i].Card] = (originIndex, previewIndex);
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            for (int i = previewTargetSlotIndex; i < (CardSlotList.Count - 1); i++)
            {
                if (CardSlotList[i].Card != null)
                {
                    int originIndex = previewCardSlotIndexDict[CardSlotList[i].Card].Item1;
                    int previewIndex = originIndex + 1;
                    previewCardSlotIndexDict[CardSlotList[i].Card] = (originIndex, previewIndex);
                }
                else
                {
                    break;
                }
            }
        }

        // 프리뷰로 이동
        foreach (var item in previewCardSlotIndexDict)
        {
            int originIndex = item.Value.Item1;
            int previewIndex = item.Value.Item2;

            Card card = item.Key;
            card.transform.DOMove(CardSlotList[previewIndex].transform.position, 0.1f);
        }
    }

    public void OnDraggableDrop(Draggable draggable)
    {
        SetBlockRaycastCardSlot(true);

        // 카드 다 백업
        Card[] tempCards = new Card[CardSlotList.Count];
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            tempCards[i] = CardSlotList[i].Card;
            CardSlotList[i].RemoveCard();
        }

        // 프리뷰에서 정해진 걸로 카드 교체
        for (int i = 0; i < tempCards.Length; i++)
        {
            if (tempCards[i] != null)
            {
                // 딕셔너리에 키가 있는지 안전하게 체크 (TryGetValue 사용)
                if (previewCardSlotIndexDict.TryGetValue(tempCards[i], out var indexTuple))
                {
                    int originIndex = indexTuple.Item1;
                    int previewIndex = indexTuple.Item2;
                    CardSlotList[previewIndex].SetCard(tempCards[originIndex]);
                }
                else
                {
                    CardSlotList[i].SetCard(tempCards[i]);
                }
            }
        }

        // 슬롯 자리 있는지 확인
        CardSlot targetSlot = CardSlotList[previewTargetSlotIndex];
        if (targetSlot.Card == null)
        {
            Card card = draggable.GetComponent<Card>();
            targetSlot.SetCard(card);
            card.transform.position = targetSlot.transform.position;
        }

        previewTargetSlotIndex = -1;

        // 수식 결과가 정답이면 엔터 버튼 활성화
        GameManager.Instance.CheckAnswerCurrentExpression();
    }

    public int FindClosestSlotIndex(Vector2 point)
    {
        int index = 0;
        float minDist = Vector2.Distance(point, CardSlotList[0].transform.position);

        for (int i = 1; i < CardSlotList.Count; i++)
        {
            float dist = Vector2.Distance(point, CardSlotList[i].transform.position);
            if (dist < minDist)
            {
                index = i;
                minDist = dist;
            }
        }

        return index;
    }

    void ResetCardVisualPosition()
    {
        previewTargetSlotIndex = -1;

        // 임시로 밀어놓은 카드를 슬롯 위치로 다시 돌려줌
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            CardSlotList[i].ResetPosition();
        }
    }

    void SetBlockRaycastCardSlot(bool isBlock)
    {
        // 다른 버튼이 가리지 않도록
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            if (CardSlotList[i].Card != null)
            {
                CardSlotList[i].Card.GetComponent<CanvasGroup>().blocksRaycasts = isBlock;
            }
        }
    }
}
