using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalculationSequenceManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject calculationResult;
    [SerializeField] TMP_Text calculationResultText;
    [SerializeField] RectTransform screenView;
    [SerializeField] AnimationCurve screenViewDownCurve; // 내려갈 때 커브
    [SerializeField] TMP_Text scoreText;

    [Header("References")]
    [SerializeField] CardSlotManager cardSlotManager;
    [SerializeField] ExpressionManager expressionManager;
    [SerializeField] ScoreManager scoreManager;

    List<CardSlot> CardSlotList => cardSlotManager.CardSlotList;

    public async UniTask PlayAsync()
    {
        List<UniTask> tempTasks = new List<UniTask>();

        // 계산하는 연출

        // 수식 카드들이 떨림
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            Card card = CardSlotList[i].Card;
            if (card != null)
            {
                tempTasks.Add(card.transform.DOShakePosition(5f, new Vector3(5f, 5f, 0f), 20).ToUniTask());
            }
        }
        // 계산 결과 천천히 오름
        calculationResult.gameObject.SetActive(true);

        // 수식 계산
        int result = expressionManager.GetExpressionResult();

        int currentVal = 0;
        tempTasks.Add(DOVirtual.Int(0, result, 5f, value =>
            {
                currentVal = value;
                calculationResultText.text = $"{currentVal}";
            })
            .SetEase(Ease.OutExpo)
            .ToUniTask()
        );
        await UniTask.WhenAll(tempTasks);
        tempTasks.Clear();

        // 정답 판별
        bool isCorrect = expressionManager.CheckAnswer(result);

        // 정답!
        //if (isCorrect)
        if (true)
        {
            // 수식 카드들 빰!
            for (int i = 0; i < CardSlotList.Count; i++)
            {
                Card card = CardSlotList[i].Card;
                if (card != null)
                {
                    tempTasks.Add(card.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
                }
            }
            // 계산 결과 빰!
            tempTasks.Add(calculationResultText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
         
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            await UniTask.WaitForSeconds(0.5f);

            // 연출을 강조하기 위해서 UI 내리기
            tempTasks.Add(screenView.DOLocalMoveY(-420f, 1.5f).SetEase(screenViewDownCurve).ToUniTask());

            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            await UniTask.WaitForSeconds(0.5f);

            // 점수 정산

            // 숫자 카드 필터링
            List<Card> tempCardList = new List<Card>();
            for (int i = 0; i < CardSlotList.Count; i++)
            {
                Card card = CardSlotList[i].Card;
                if (card != null)
                {
                    if (card.CardData.CardCategory == CardCategory.Number)
                    {
                        tempCardList.Add(card);
                    }
                }
            }

            // 위로 올려줌
            for (int i = 0; i < tempCardList.Count; i++)
            {
                Card card = tempCardList[i];
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            await UniTask.WaitForSeconds(0.5f);

            // 하나씩 정산
            for (int i = 0; i < tempCardList.Count; i++)
            {
                Card card = tempCardList[i];

                UniTask.Void(async () =>
                {
                    await UniTask.WaitForSeconds(0.07f);

                    int bonus = 0;
                    switch (card.CardData.CardType)
                    {
                        case CardType.None: break;
                        case CardType.Number1: bonus = 1; break;
                        case CardType.Number2: bonus = 2; break;
                        case CardType.Number3: bonus = 3; break;
                        case CardType.Number4: bonus = 4; break;
                        case CardType.Number5: bonus = 5; break;
                        case CardType.Number6: bonus = 6; break;
                        case CardType.Number7: bonus = 7; break;
                        case CardType.Number8: bonus = 8; break;
                        case CardType.Number9: bonus = 9; break;
                        default:
                            break;
                    }
                    scoreManager.AddNumberCardBonus(bonus);
                });

                await card.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f);
            }

            await UniTask.WaitForSeconds(0.5f);

            // 내려줌
            for (int i = 0; i < tempCardList.Count; i++)
            {
                Card card = tempCardList[i];
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            tempCardList.Clear();

            // 연산자 카드 (+, -) 필터링
            for (int i = 0; i < CardSlotList.Count; i++)
            {
                Card card = CardSlotList[i].Card;
                if (card != null)
                {
                    if (card.CardData.CardCategory == CardCategory.Operator)
                    {
                        if (card.CardData.CardType == CardType.Plus || card.CardData.CardType == CardType.Minus)
                        {
                            tempCardList.Add(card);
                        }
                    }
                }
            }

            // 위로 올려줌
            for (int i = 0; i < tempCardList.Count; i++)
            {
                Card card = tempCardList[i];
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            await UniTask.WaitForSeconds(0.5f);

            // 하나씩 정산
            for (int i = 0; i < tempCardList.Count; i++)
            {
                Card card = tempCardList[i];

                UniTask.Void(async () =>
                {
                    await UniTask.WaitForSeconds(0.07f);
                    scoreManager.AddPlusMinusCardBonus(1);
                });

                await card.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f);

            }

            await UniTask.WaitForSeconds(0.5f);

            // 내려줌
            for (int i = 0; i < tempCardList.Count; i++)
            {
                Card card = tempCardList[i];
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            tempCardList.Clear();

            // 연산자 카드 (*) 필터링
            for (int i = 0; i < CardSlotList.Count; i++)
            {
                Card card = CardSlotList[i].Card;
                if (card != null)
                {
                    if (card.CardData.CardCategory == CardCategory.Operator)
                    {
                        if (card.CardData.CardType == CardType.Multiply)
                        {
                            tempCardList.Add(card);
                        }
                    }
                }
            }

            // 위로 올려줌
            for (int i = 0; i < tempCardList.Count; i++)
            {
                Card card = tempCardList[i];
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            await UniTask.WaitForSeconds(0.5f);

            // 하나씩 정산
            for (int i = 0; i < tempCardList.Count; i++)
            {
                Card card = tempCardList[i];

                UniTask.Void(async () =>
                {
                    await UniTask.WaitForSeconds(0.07f);
                    scoreManager.MulMulCardBonus(2);
                });

                await card.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f);

            }

            await UniTask.WaitForSeconds(0.5f);

            // 내려줌
            for (int i = 0; i < tempCardList.Count; i++)
            {
                Card card = tempCardList[i];
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            tempCardList.Clear();

            // 공격 연출
            await UniTask.WaitForSeconds(3);

            // 보드 정리하기

            // 다시 UI 올리기
            tempTasks.Add(screenView.DOLocalMoveY(0f, 1.5f).SetEase(Ease.OutQuart).ToUniTask());
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();
        }

        await UniTask.WaitForSeconds(3);
        calculationResult.gameObject.SetActive(false);

        await UniTask.CompletedTask;
    }
}
