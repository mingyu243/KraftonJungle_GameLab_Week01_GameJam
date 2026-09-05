using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("InGame")]
    public bool IsDie = false;
    public bool IsInputComplete = false;
    public int TargetNumber;
    public int StageLevel;
    public int Score;

    [Header("UI")]
    [SerializeField] TMP_Text targetNumberText;
    [SerializeField] GameObject enterButton;

    [Header("References")]
    [SerializeField] CardSlotManager cardSlotManager;
    [SerializeField] CardBoardManager cardBoardManager;
    [SerializeField] CalculationSequenceManager calculationSequenceManager;

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
            // 초기화
            IsDie = false;
            StageLevel = 1;
            Score = 0;

            // 플레이어 등장

            // 플레이어가 죽을 때까지 스테이지 반복
            while (!IsDie)
            {
                IsInputComplete = false;

                // 적 등장


                // 카드 깔기
                cardBoardManager.SetupCards();

                // 랜덤 숫자 지정
                TargetNumber = 100;
                targetNumberText.text = $"{TargetNumber} 만들기";

                // 버튼 활성화
                enterButton.SetActive(true);

                // 플레이어 입력 판정이 들어올 때까지 대기
                await UniTask.WaitUntil(() => IsInputComplete);

                // 버튼 비활성화
                enterButton.SetActive(false);

                // 연출
                await calculationSequenceManager.PlayAsync();
            }

            // 게임 오버

            await UniTask.CompletedTask;
        });
    }
}
