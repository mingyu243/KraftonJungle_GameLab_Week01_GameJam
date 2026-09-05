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
        return Instantiate(cardPrefab);
    }
    public Card Spawn(CardData cardData)
    {
        return null;
    }
}
