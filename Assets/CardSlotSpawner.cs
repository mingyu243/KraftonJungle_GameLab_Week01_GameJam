using System.Collections.Generic;
using UnityEngine;

public class CardSlotSpawner : MonoBehaviour
{
    [SerializeField] CardSlot cardSlotPrefab;
    [Space]
    [SerializeField] Transform slotParentTr;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [Space]
    [SerializeField] int spawnCount = 13;

    public List<CardSlot> SetupCardSlots()
    {
        List<CardSlot> newCardSlotList = new();

        // 슬롯 생성 위치 계산
        float startPointX = startPoint.position.x;
        float endPointX = endPoint.position.x;

        for (int i = 0; i < spawnCount; i++)
        {
            float gap = (endPointX - startPointX) / (spawnCount - 1);
            Vector2 point = startPoint.position + new Vector3(gap * i, 0, 0);

            // 슬롯 생성
            CardSlot newCardSlot = Instantiate(cardSlotPrefab, point, Quaternion.identity, slotParentTr);
            newCardSlotList.Add(newCardSlot);
        }

        return newCardSlotList;
    }
}
