using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] Card cardPrefab;

    [SerializeField] Transform spawnParentTr;
    [SerializeField] Transform startPoint; // 첫줄 가로 시작
    [SerializeField] Transform endPoint; // 첫줄 가로 끝
    [Space]
    [SerializeField] int spawnColCount = 14;
    [SerializeField] int spawnRowCount = 2;
    [SerializeField] float rowSpacing = 120;

    public void SetupCards()
    {
        float colGap = 0f;
        if (spawnColCount > 1)
        {
            colGap = (endPoint.position.x - startPoint.position.x) / (spawnColCount - 1);
        }

        for (int r = 0; r < spawnRowCount; r++)
        {
            for (int c = 0; c < spawnColCount; c++)
            {
                Vector3 spawnPos = startPoint.position + new Vector3(colGap * c, -rowSpacing * r, 0);

                Card newCard = SpawnRandom();
                newCard.transform.SetParent(spawnParentTr);
                newCard.transform.position = spawnPos;
            }
        }
    }

    public Card SpawnRandom()
    {
        return Spawn(CreateRandomCardData());
    }

    public Card Spawn(CardData newCardData)
    {
        Card newCard = Instantiate(cardPrefab);
        newCard.CardData = newCardData;
        newCard.UpdateUI();

        return newCard;
    }

    CardData CreateRandomCardData()
    {
        CardData newCardData = new CardData();

        int category = Random.Range(0, 2);
        if (category == 0)
        {
            newCardData.CardCategory = CardCategory.Number;

            int type = Random.Range(1, 10);
            newCardData.CardType = (CardType)type;
        }
        else
        {
            newCardData.CardCategory = CardCategory.Operator;

            int type = Random.Range(10, 13);
            newCardData.CardType = (CardType)type;
        }

        return newCardData;
    }
}
