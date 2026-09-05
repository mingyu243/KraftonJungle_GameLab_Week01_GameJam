using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] CardSlotsManager cardSlotsManager;
    [SerializeField] CardBoardManager cardBoardManager;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        UniTask.Void(async () =>
        {
            // 카드 깔기
            cardBoardManager.SetupCards();

            // 플레이어가 수식 완료하거나 죽는 조건이 될 때까지 대기

            // 계산 연출

            // 공격 연출

            // 적이 죽으면 다시 생성

            // 다시 죽을때까지 반복

            await UniTask.CompletedTask;
        });
    }
}
