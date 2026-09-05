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
}
