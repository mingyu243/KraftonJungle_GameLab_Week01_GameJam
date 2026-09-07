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
    [SerializeField] TMP_Text scoreText;

    [Header("References")]
    [SerializeField] CardSlotManager cardSlotManager;
    [SerializeField] CardBoardManager cardBoardManager;
    [SerializeField] ExpressionManager expressionManager;
    [SerializeField] ScoreManager scoreManager;

    List<CardSlot> CardSlotList => cardSlotManager.CardSlotList;

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

            await UniTask.WaitForSeconds(1.5f);

            // 잠시 내려갔다가
            await screenView.DOAnchorPosY(0f, 1.5f).ToUniTask();

            // 정리
            calculationResult.gameObject.SetActive(false);
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

    public async UniTask ScoreCalculationAsync()
    {
        List<UniTask> tasks = new List<UniTask>();

        // 점수 정산

        // 숫자 카드 필터링
        List<CardSlot> tempCardSlotList = new List<CardSlot>();
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            CardSlot cardSlot = CardSlotList[i];
            Card card = cardSlot.Card;
            if (card != null)
            {
                if (card.CardData.CardCategory == CardCategory.Number)
                {
                    tempCardSlotList.Add(cardSlot);
                }
            }
        }
        if (tempCardSlotList.Count > 0)
        {
            // 위로 올려줌
            for (int i = 0; i < tempCardSlotList.Count; i++)
            {
                CardSlot cardSlot = tempCardSlotList[i];
                Card card = cardSlot.Card;
                tasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
                tasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tasks);
            tasks.Clear();

            await UniTask.WaitForSeconds(0.5f);

            // 하나씩 정산
            for (int i = 0; i < tempCardSlotList.Count; i++)
            {
                CardSlot cardSlot = tempCardSlotList[i];
                Card card = cardSlot.Card;

                UniTask.Void(async () =>
                {
                    await UniTask.WaitForSeconds(0.09f);

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
                    scoreManager.AddNumberCardBonus(bonus, cardSlot.transform);
                    player.Atk = scoreManager.Score;
                });

                tasks.Add(cardSlot.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
                tasks.Add(card.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());

                await UniTask.WhenAll(tasks);
                tasks.Clear();
            }

            await UniTask.WaitForSeconds(0.5f);

            // 내려줌
            for (int i = 0; i < tempCardSlotList.Count; i++)
            {
                CardSlot cardSlot = tempCardSlotList[i];
                Card card = cardSlot.Card;
                tasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
                tasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tasks);
            tasks.Clear();

            tempCardSlotList.Clear();
        }

        // 연산자 카드 필터링
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            CardSlot cardSlot = CardSlotList[i];
            Card card = cardSlot.Card;
            if (card != null)
            {
                if (card.CardData.CardCategory == CardCategory.Operator)
                {
                    tempCardSlotList.Add(cardSlot);
                }
            }
        }
        if (tempCardSlotList.Count > 0)
        {
            // 위로 올려줌
            for (int i = 0; i < tempCardSlotList.Count; i++)
            {
                CardSlot cardSlot = tempCardSlotList[i];
                Card card = cardSlot.Card;
                tasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
                tasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y + 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tasks);
            tasks.Clear();

            await UniTask.WaitForSeconds(0.5f);

            // 하나씩 정산
            for (int i = 0; i < tempCardSlotList.Count; i++)
            {
                CardSlot cardSlot = tempCardSlotList[i];
                Card card = cardSlot.Card;

                UniTask.Void(async () =>
                {
                    await UniTask.WaitForSeconds(0.09f);
                    scoreManager.MulOperatorCardBonus(2, cardSlot.transform);
                    player.Atk = scoreManager.Score;
                });

                tasks.Add(cardSlot.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());
                tasks.Add(card.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, vibrato: 1, elasticity: 1f).ToUniTask());

                await UniTask.WhenAll(tasks);
                tasks.Clear();
            }

            await UniTask.WaitForSeconds(0.3f);

            // 내려줌
            for (int i = 0; i < tempCardSlotList.Count; i++)
            {
                CardSlot cardSlot = tempCardSlotList[i];
                Card card = cardSlot.Card;
                tasks.Add(cardSlot.transform.DOLocalMoveY(cardSlot.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
                tasks.Add(card.transform.DOLocalMoveY(card.transform.localPosition.y - 50, 0.2f).SetDelay(i * 0.06f).ToUniTask());
            }
            await UniTask.WhenAll(tasks);
        }
    }

    public async UniTask ExpressionCorrectAsync()
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

    public async UniTask FocusActionAsync()
    {
        // 연출을 강조하기 위해서 UI 내리기
        await screenView.DOAnchorPosY(180f, 1.5f).SetEase(screenViewDownCurve).ToUniTask();
    }

    public async UniTask PlayActionAsync()
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
}
