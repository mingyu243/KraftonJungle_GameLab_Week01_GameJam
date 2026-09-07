using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalculationSequenceManager : MonoBehaviour
{
    [Header("Action")]
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    [SerializeField] RectTransform shakePanelTr;

    [Header("UI")]
    [SerializeField] GameObject calculationResult;
    [SerializeField] TMP_Text calculationResultText;
    [SerializeField] RectTransform screenView;
    [SerializeField] AnimationCurve screenViewDownCurve; // 내려갈 때 커브
    [SerializeField] GameObject[] scoreBonusDescs;

    [Header("References")]
    [SerializeField] CardSlotManager cardSlotManager;
    [SerializeField] CardBoardManager cardBoardManager;
    [SerializeField] ExpressionManager expressionManager;
    [SerializeField] ScoreManager scoreManager;

    List<CardSlot> CardSlotList => cardSlotManager.CardSlotList;

    void Start()
    {
        for (int i = 0; i < scoreBonusDescs.Length; i++)
        {
            scoreBonusDescs[i].SetActive(false);
        }
    }

    public async UniTask PlayAsync()
    {
        int result = await ExpressionCalculationAsync();

        bool isCorrect = expressionManager.CheckAnswer(result);

        //if (isCorrect)
        if (true)
        {
            await ExpressionCorrectAsync();

            await UniTask.WaitForSeconds(0.5f);

            await FocusActionAsync();

            await UniTask.WaitForSeconds(0.5f);

            await ScoreCalculationAsync();

            await UniTask.WaitForSeconds(0.5f);

            await PlayActionAsync();

            await UniTask.WaitForSeconds(1.2f);

            // 정리
            calculationResult.gameObject.SetActive(false);

            for (int i = 0; i < scoreBonusDescs.Length; i++)
            {
                scoreBonusDescs[i].SetActive(false);
            }

            // 잠시 내려갔다가
            await screenView.DOAnchorPosY(0f, 1.5f).ToUniTask();
        }

        await UniTask.CompletedTask;
    }

    public async UniTask<int> ExpressionCalculationAsync()
    {
        List<UniTask> tasks = new List<UniTask>();

        // 수식 카드들이 떨림
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            Card card = CardSlotList[i].Card;
            if (card != null)
            {
                tasks.Add(card.transform.DOShakePosition(3f, new Vector3(5f, 5f, 0f), 20).ToUniTask());
            }
        }
        // 계산 결과 천천히 오름
        calculationResult.gameObject.SetActive(true);

        // 수식 계산
        int result = expressionManager.GetExpressionResult();

        int currentVal = 0;
        tasks.Add(DOVirtual.Int(0, result, 3f, value =>
        {
            currentVal = value;
            calculationResultText.text = $"{currentVal}";
        })
            .SetEase(Ease.OutExpo)
            .ToUniTask()
        );
        await UniTask.WhenAll(tasks);

        return result;
    }

    async UniTask ScoreCalculationAsync()
    {
        List<UniTask> tasks = new List<UniTask>();

        // 점수 정산
        await ScoreFullSlotAsync();
        await ScoreNumberAsync();
        await ScoreOperatorPlusMinusAsync();
        await ScoreOperatorMulAsync();
        await ScorePairAsync();
        await ScoreTripleAsync();
    }

    async UniTask ExpressionCorrectAsync()
    {
        List<UniTask> tasks = new List<UniTask>();

        // 수식 카드들 빰!
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            Card card = CardSlotList[i].Card;
            if (card != null)
            {
                tasks.Add(card.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
            }
        }
        // 계산 결과 빰!
        tasks.Add(calculationResultText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());

        await UniTask.WhenAll(tasks);
    }

    async UniTask FocusActionAsync()
    {
        // 연출을 강조하기 위해서 UI 내리기
        await screenView.DOAnchorPosY(180f, 1.5f).SetEase(screenViewDownCurve).ToUniTask();
    }

    async UniTask PlayActionAsync()
    {
        // 플레이어 공격
        int playerPower = scoreManager.Score;
        await player.AttackAnimAsync();

        enemy.Health = enemy.Health - playerPower;

        if (enemy.Health <= 0)
        {
            // 타격감
            UniTask.Void(async () =>
            {
                Time.timeScale = 0.02f;
                shakePanelTr.DOShakeAnchorPos(0.15f, new Vector3(15f, 15f, 0f), 20, 90f).SetUpdate(true).ToUniTask().Forget();
                await UniTask.WaitForSeconds(0.7f, true);
                Time.timeScale = 1f;
            });
            await enemy.DieAnimAsync();
        }
        else
        {
            // 타격감
            UniTask.Void(async () =>
            {
                Time.timeScale = 0.02f;
                shakePanelTr.DOShakeAnchorPos(0.15f, new Vector3(15f, 15f, 0f), 20, 90f).SetUpdate(true).ToUniTask().Forget();
                await UniTask.WaitForSeconds(0.3f, true);
                Time.timeScale = 1f;
            });
            await enemy.TakeDamageAnimAsync(playerPower);

            // 적 공격
            await enemy.AttackAnimAsync();

            player.Health -= 1;
            GameManager.Instance.Life = player.Health;

            // 플레이어 사망
            if (player.Health <= 0)
            {
                // 타격감
                UniTask.Void(async () =>
                {
                    Time.timeScale = 0.02f;
                    shakePanelTr.DOShakeAnchorPos(0.15f, new Vector3(15f, 15f, 0f), 20, 90f).SetUpdate(true).ToUniTask().Forget();
                    await UniTask.WaitForSeconds(0.7f, true);
                    Time.timeScale = 1f;
                });
                await player.DieAnimAsync();
            }
            else
            {
                // 타격감
                UniTask.Void(async () =>
                {
                    Time.timeScale = 0.02f;
                    shakePanelTr.DOShakeAnchorPos(0.15f, new Vector3(15f, 15f, 0f), 20, 90f).SetUpdate(true).ToUniTask().Forget();
                    await UniTask.WaitForSeconds(0.3f, true);
                    Time.timeScale = 1f;
                });
                await player.TakeDamageAnimAsync();
            }
        }
    }

    async UniTask ScoreFullSlotAsync()
    {
        List<UniTask> tempTasks = new List<UniTask>();
        List<CardSlot> cardSlotList = new List<CardSlot>();

        // 슬롯 다 채웠는지 필터링
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            CardSlot cardSlot = CardSlotList[i];
            Card card = cardSlot.Card;
            if (card != null)
            {
                cardSlotList.Add(cardSlot);
            }
        }
        if (cardSlotList.Count == cardSlotManager.CardSlotList.Count)
        {
            scoreBonusDescs[0].SetActive(true);
            // 위로 올려줌
            for (int i = 0; i < cardSlotList.Count; i++)
            {
                CardSlot cardSlot = cardSlotList[i];
                Card card = cardSlot.Card;
                tempTasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y + 50, 0.2f).ToUniTask());
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y + 50, 0.2f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            await UniTask.WaitForSeconds(0.3f);

            // 정산
            UniTask.Void(async () =>
            {
                await UniTask.WaitForSeconds(0.09f);

                scoreManager.AddMulBonus(GameManager.Instance.ScoreFullSlotAddMulBonus, cardSlotList[cardSlotList.Count / 2].transform);
                player.Atk = scoreManager.Score;
            });
            for (int i = 0; i < cardSlotList.Count; i++)
            {
                CardSlot cardSlot = cardSlotList[i];
                Card card = cardSlot.Card;
                tempTasks.Add(cardSlot.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
                tempTasks.Add(card.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            await UniTask.WaitForSeconds(0.5f);

            // 내려줌
            for (int i = 0; i < cardSlotList.Count; i++)
            {
                CardSlot cardSlot = cardSlotList[i];
                Card card = cardSlot.Card;
                tempTasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y - 50, 0.2f).ToUniTask());
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y - 50, 0.2f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
        }
    }

    async UniTask ScoreNumberAsync()
    {
        List<UniTask> tempTasks = new List<UniTask>();
        List<CardSlot> cardSlotList = new List<CardSlot>();

        // 숫자 카드 필터링
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            CardSlot cardSlot = CardSlotList[i];
            Card card = cardSlot.Card;
            if (card != null)
            {
                if (card.CardData.CardCategory == CardCategory.Number)
                {
                    cardSlotList.Add(cardSlot);
                }
            }
        }
        if (cardSlotList.Count > 0)
        {
            scoreBonusDescs[1].SetActive(true);
            // 위로 올려줌
            for (int i = 0; i < cardSlotList.Count; i++)
            {
                CardSlot cardSlot = cardSlotList[i];
                Card card = cardSlot.Card;
                tempTasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            await UniTask.WaitForSeconds(0.5f);

            // 하나씩 정산
            for (int i = 0; i < cardSlotList.Count; i++)
            {
                CardSlot cardSlot = cardSlotList[i];
                Card card = cardSlot.Card;

                UniTask.Void(async () =>
                {
                    await UniTask.WaitForSeconds(0.09f);

                    int bonus = (int)card.CardData.CardType;
                    scoreManager.AddSumBonus(bonus, cardSlot.transform);
                    player.Atk = scoreManager.Score;
                });

                tempTasks.Add(cardSlot.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
                tempTasks.Add(card.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());

                await UniTask.WhenAll(tempTasks);
                tempTasks.Clear();
            }

            await UniTask.WaitForSeconds(0.5f);

            // 내려줌
            for (int i = 0; i < cardSlotList.Count; i++)
            {
                CardSlot cardSlot = cardSlotList[i];
                Card card = cardSlot.Card;
                tempTasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
        }
    }

    async UniTask ScoreOperatorPlusMinusAsync()
    {
        List<UniTask> tempTasks = new List<UniTask>();
        List<CardSlot> cardSlotList = new List<CardSlot>();

        // 연산자 카드 +, - 필터링
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            CardSlot cardSlot = CardSlotList[i];
            Card card = cardSlot.Card;
            if (card != null)
            {
                if (card.CardData.CardCategory == CardCategory.Operator)
                {
                    if (card.CardData.CardType == CardType.Plus || card.CardData.CardType == CardType.Minus)
                    {
                        cardSlotList.Add(cardSlot);
                    }
                }
            }
        }
        if (cardSlotList.Count > 0)
        {
            scoreBonusDescs[2].SetActive(true);
            // 위로 올려줌
            for (int i = 0; i < cardSlotList.Count; i++)
            {
                CardSlot cardSlot = cardSlotList[i];
                Card card = cardSlot.Card;
                tempTasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            await UniTask.WaitForSeconds(0.5f);

            // 하나씩 정산
            for (int i = 0; i < cardSlotList.Count; i++)
            {
                CardSlot cardSlot = cardSlotList[i];
                Card card = cardSlot.Card;

                UniTask.Void(async () =>
                {
                    await UniTask.WaitForSeconds(0.09f);
                    scoreManager.AddMulBonus(GameManager.Instance.ScorePlusMinusAddMulBonus, cardSlot.transform);
                    player.Atk = scoreManager.Score;
                });

                tempTasks.Add(cardSlot.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
                tempTasks.Add(card.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());

                await UniTask.WhenAll(tempTasks);
                tempTasks.Clear();
            }

            await UniTask.WaitForSeconds(0.3f);

            // 내려줌
            for (int i = 0; i < cardSlotList.Count; i++)
            {
                CardSlot cardSlot = cardSlotList[i];
                Card card = cardSlot.Card;
                tempTasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
        }
    }

    async UniTask ScoreOperatorMulAsync()
    {
        List<UniTask> tempTasks = new List<UniTask>();
        List<CardSlot> cardSlotList = new List<CardSlot>();

        // 연산자 카드 * 필터링
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            CardSlot cardSlot = CardSlotList[i];
            Card card = cardSlot.Card;
            if (card != null)
            {
                if (card.CardData.CardCategory == CardCategory.Operator)
                {
                    if (card.CardData.CardType == CardType.Multiply)
                    {
                        cardSlotList.Add(cardSlot);
                    }
                }
            }
        }
        if (cardSlotList.Count > 0)
        {
            scoreBonusDescs[3].SetActive(true);
            // 위로 올려줌
            for (int i = 0; i < cardSlotList.Count; i++)
            {
                CardSlot cardSlot = cardSlotList[i];
                Card card = cardSlot.Card;
                tempTasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
            tempTasks.Clear();

            await UniTask.WaitForSeconds(0.5f);

            // 하나씩 정산
            for (int i = 0; i < cardSlotList.Count; i++)
            {
                CardSlot cardSlot = cardSlotList[i];
                Card card = cardSlot.Card;

                UniTask.Void(async () =>
                {
                    await UniTask.WaitForSeconds(0.09f);
                    scoreManager.MulMulBonus(GameManager.Instance.ScoreMulMulMulBonus, cardSlot.transform);
                    player.Atk = scoreManager.Score;
                });

                tempTasks.Add(cardSlot.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
                tempTasks.Add(card.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());

                await UniTask.WhenAll(tempTasks);
                tempTasks.Clear();
            }

            await UniTask.WaitForSeconds(0.3f);

            // 내려줌
            for (int i = 0; i < cardSlotList.Count; i++)
            {
                CardSlot cardSlot = cardSlotList[i];
                Card card = cardSlot.Card;
                tempTasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
                tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tempTasks);
        }
    }

    async UniTask ScorePairAsync()
    {
        List<UniTask> tempTasks = new List<UniTask>();
        Dictionary<int, List<CardSlot>> numberGroups = new Dictionary<int, List<CardSlot>>();

        // Pair 필터링
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            CardSlot cardSlot = CardSlotList[i];
            Card card = cardSlot.Card;
            if (card != null && card.CardData.CardCategory == CardCategory.Number)
            {
                int numberValue = (int)card.CardData.CardType;
                if (!numberGroups.ContainsKey(numberValue))
                {
                    numberGroups[numberValue] = new List<CardSlot>();
                }
                numberGroups[numberValue].Add(cardSlot);
            }
        }
        List<List<CardSlot>> pairGroups = new List<List<CardSlot>>();
        foreach (var group in numberGroups.Values)
        {
            if (group.Count == 2)
            {
                pairGroups.Add(group);
            }
        }

        if (pairGroups.Count > 0)
        {
            scoreBonusDescs[4].SetActive(true);

            // Pair 여러 개 순차적으로
            foreach (var pairGroup in pairGroups)
            {
                // 한번에 위로 올려줌
                for (int i = 0; i < pairGroup.Count; i++)
                {
                    CardSlot cardSlot = pairGroup[i];
                    Card card = cardSlot.Card;
                    tempTasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y + 50, 0.2f).ToUniTask());
                    tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y + 50, 0.2f).ToUniTask());
                }
                await UniTask.WhenAll(tempTasks);
                tempTasks.Clear();

                await UniTask.WaitForSeconds(0.3f);

                // 한번에 정산
                UniTask.Void(async () =>
                {
                    await UniTask.WaitForSeconds(0.09f);
                    scoreManager.MulMulBonus(GameManager.Instance.ScorePairMulMulBonus, pairGroup[0].transform);
                    player.Atk = scoreManager.Score;
                });
                for (int i = 0; i < pairGroup.Count; i++)
                {
                    CardSlot cardSlot = pairGroup[i];
                    Card card = cardSlot.Card;
                    tempTasks.Add(cardSlot.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
                    tempTasks.Add(card.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
                }
                await UniTask.WhenAll(tempTasks);
                tempTasks.Clear();

                await UniTask.WaitForSeconds(0.4f);

                // 한번에 내려줌
                for (int i = 0; i < pairGroup.Count; i++)
                {
                    CardSlot cardSlot = pairGroup[i];
                    Card card = cardSlot.Card;
                    tempTasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y - 50, 0.2f).ToUniTask());
                    tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y - 50, 0.2f).ToUniTask());
                }
                await UniTask.WhenAll(tempTasks);
                tempTasks.Clear();

                // 페어와 페어 사이의 간격
                await UniTask.WaitForSeconds(0.2f);
            }
        }
    }

    async UniTask ScoreTripleAsync()
    {
        List<UniTask> tempTasks = new List<UniTask>();
        Dictionary<int, List<CardSlot>> numberGroups = new Dictionary<int, List<CardSlot>>();

        // Pair 필터링
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            CardSlot cardSlot = CardSlotList[i];
            Card card = cardSlot.Card;
            if (card != null && card.CardData.CardCategory == CardCategory.Number)
            {
                int numberValue = (int)card.CardData.CardType;
                if (!numberGroups.ContainsKey(numberValue))
                {
                    numberGroups[numberValue] = new List<CardSlot>();
                }
                numberGroups[numberValue].Add(cardSlot);
            }
        }
        List<List<CardSlot>> pairGroups = new List<List<CardSlot>>();
        foreach (var group in numberGroups.Values)
        {
            if (group.Count == 3)
            {
                pairGroups.Add(group);
            }
        }

        if (pairGroups.Count > 0)
        {
            scoreBonusDescs[5].SetActive(true);

            // Pair 여러 개 순차적으로
            foreach (var pairGroup in pairGroups)
            {
                // 한번에 위로 올려줌
                for (int i = 0; i < pairGroup.Count; i++)
                {
                    CardSlot cardSlot = pairGroup[i];
                    Card card = cardSlot.Card;
                    tempTasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y + 50, 0.2f).ToUniTask());
                    tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y + 50, 0.2f).ToUniTask());
                }
                await UniTask.WhenAll(tempTasks);
                tempTasks.Clear();

                await UniTask.WaitForSeconds(0.3f);

                // 한번에 정산
                UniTask.Void(async () =>
                {
                    await UniTask.WaitForSeconds(0.09f);
                    scoreManager.MulMulBonus(GameManager.Instance.ScoreTripleMulMulBonus, pairGroup[0].transform);
                    player.Atk = scoreManager.Score;
                });
                for (int i = 0; i < pairGroup.Count; i++)
                {
                    CardSlot cardSlot = pairGroup[i];
                    Card card = cardSlot.Card;
                    tempTasks.Add(cardSlot.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
                    tempTasks.Add(card.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
                }
                await UniTask.WhenAll(tempTasks);
                tempTasks.Clear();

                await UniTask.WaitForSeconds(0.4f);

                // 한번에 내려줌
                for (int i = 0; i < pairGroup.Count; i++)
                {
                    CardSlot cardSlot = pairGroup[i];
                    Card card = cardSlot.Card;
                    tempTasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y - 50, 0.2f).ToUniTask());
                    tempTasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y - 50, 0.2f).ToUniTask());
                }
                await UniTask.WhenAll(tempTasks);
                tempTasks.Clear();

                // 페어와 페어 사이의 간격
                await UniTask.WaitForSeconds(0.2f);
            }
        }
    }
}
