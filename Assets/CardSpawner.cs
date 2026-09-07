using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] Card cardPrefab;

    [SerializeField] Transform spawnParentTr;
    [SerializeField] RectTransform startPoint; // 첫줄 가로 시작
    [SerializeField] RectTransform endPoint; // 첫줄 가로 끝
    [Space]
    [SerializeField] int spawnColCount = 14;
    [SerializeField] int spawnRowCount = 2;
    [SerializeField] float rowSpacing = 145;

    public List<Card> SetupCards()
    {
        // 카드 생성
        List<Card> newCardList = new();

        // 최소 보장
        newCardList.Add(Spawn(new CardData() { CardCategory = CardCategory.Number, CardType = (CardType)1 }));
        for (int i = 0; i < 2; i++)
        {
            // 숫자
            for (int j = 2; j <= 9; j++)
            {
                newCardList.Add(Spawn(new CardData() { CardCategory = CardCategory.Number, CardType = (CardType)j }));
            }

            // 연산자
            newCardList.Add(Spawn(new CardData() { CardCategory = CardCategory.Operator, CardType = CardType.Plus }));
            newCardList.Add(Spawn(new CardData() { CardCategory = CardCategory.Operator, CardType = CardType.Minus }));
            newCardList.Add(Spawn(new CardData() { CardCategory = CardCategory.Operator, CardType = CardType.Multiply }));
        }

        // 랜덤 생성
        for (int i = newCardList.Count; i < spawnRowCount * spawnColCount; i++)
        {
            if (i % 2 == 0)
            {
                newCardList.Add(Spawn(CreateRandomNumberCardData()));
            }
            else
            {
                newCardList.Add(Spawn(CreateRandomOperatorCardData()));
            }
        }

        newCardList = newCardList.OrderBy(x => Random.value).ToList();

        // 배치
        float colGap = 0f;
        if (spawnColCount > 1)
        {
            colGap = (endPoint.anchoredPosition.x - startPoint.anchoredPosition.x) / (spawnColCount - 1);
        }

        for (int r = 0; r < spawnRowCount; r++)
        {
            for (int c = 0; c < spawnColCount; c++)
            {
                Card card = newCardList[(r * spawnColCount) + c];

                Vector3 spawnPos = startPoint.anchoredPosition + new Vector2(colGap * c, -rowSpacing * r);
                card.GetComponent<RectTransform>().anchoredPosition = spawnPos;
            }
        }

        return newCardList;
    }

    public Card Spawn(CardData newCardData)
    {
        Card newCard = Instantiate(cardPrefab);
        newCard.transform.SetParent(spawnParentTr);
        newCard.transform.localScale = Vector3.one;
        newCard.CardData = newCardData;
        newCard.UpdateUI();

        return newCard;
    }

    CardData CreateRandomNumberCardData()
    {
        CardData newCardData = new CardData();
        newCardData.CardCategory = CardCategory.Number;

        int type = Random.Range(2, 10);
        newCardData.CardType = (CardType)type;

        return newCardData;
    }

    CardData CreateRandomOperatorCardData()
    {
        CardData newCardData = new CardData();
        newCardData.CardCategory = CardCategory.Operator;

        int type = Random.Range(10, 13);
        newCardData.CardType = (CardType)type;

        return newCardData;
    }
}
