using System;
using System.Collections.Generic;
using UnityEngine;

public class CardBoardManager : MonoBehaviour
{
    public List<Card> CardList = new();

    [Header("References")]
    [SerializeField] CardSpawner cardSpawner;

    public void SetupCards()
    {
        CardList = cardSpawner.SetupCards();
    }

    public void Clear()
    {
        for (int i = 0; i < CardList.Count; i++)
        {
            Card card = CardList[i];

            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }

        CardList.Clear();
    }
}