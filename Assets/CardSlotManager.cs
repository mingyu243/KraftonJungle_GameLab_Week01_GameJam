using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotManager : MonoBehaviour
{
    public List<CardSlot> CardSlotList = new();

    [Header("References")]
    [SerializeField] CardSlotSpawner cardSlotSpawner;

    private void Start()
    {
        CardSlotList = cardSlotSpawner.SetupCardSlots();
    }

    public void Clear()
    {
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            CardSlot cardSlot = CardSlotList[i];
            Card card = cardSlot.Card;
            
            if (card != null)
            {
                cardSlot.RemoveCard(destroyCardObject: true);
            }
        }
    }
}
