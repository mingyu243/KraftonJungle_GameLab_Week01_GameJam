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


    [Header("References")]
    [SerializeField] CardSlotManager cardSlotManager;
    [SerializeField] ExpressionManager expressionManager;

    List<CardSlot> CardSlotList => cardSlotManager.CardSlotList;

    public async UniTask PlayAsync()
    {
        List<UniTask> tasks = new List<UniTask>();

        // 계산하는 연출 
        // 수식 카드들이 떨림
        for (int i = 0; i < CardSlotList.Count; i++)
        {
            Card card = CardSlotList[i].Card;
            if (card != null)
            {
                tasks.Add(card.transform.DOShakePosition(5f, new Vector3(5f, 5f, 0f), 20).ToUniTask());
            }
        }
        // 계산 결과 천천히 오름
        calculationResult.gameObject.SetActive(true);

        // 수식 계산
        int result = expressionManager.GetExpressionResult();

        int currentVal = 0;
        tasks.Add(DOVirtual.Int(0, result, 5f, value =>
            {
                currentVal = value;
                calculationResultText.text = $"{currentVal}";
            })
            .SetEase(Ease.OutExpo)
            .ToUniTask()
        );
        await UniTask.WhenAll(tasks);
        tasks.Clear();

        // 정답 판별
        bool isCorrect = expressionManager.CheckAnswer(result);

        // 정답!
        if (isCorrect)
        {
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
            tasks.Clear();

            await UniTask.WaitForSeconds(0.5f);

            // 연출을 강조하기 위해서 UI 내리기
            tasks.Add(screenView.DOLocalMoveY(-420f, 1.5f).SetEase(screenViewDownCurve).ToUniTask());
            await UniTask.WhenAll(tasks);
            tasks.Clear();

            // 공격 연출
            await UniTask.WaitForSeconds(3);

            // 다시 UI 올리기
            tasks.Add(screenView.DOLocalMoveY(0f, 1.5f).ToUniTask());
            await UniTask.WhenAll(tasks);
            tasks.Clear();
        }

        await UniTask.WaitForSeconds(3);
        calculationResult.gameObject.SetActive(false);

        await UniTask.CompletedTask;
    }
}
