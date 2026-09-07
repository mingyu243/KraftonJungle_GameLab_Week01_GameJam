using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    public int Life = 3;
    [Range(0, 3)] public int TimeScale = 1;

    [Header("InGame")]
    public bool IsInputExpressionComplete = false;
    public int TargetNumber;
    public int StageLevel;

    [Header("UI")]
    [SerializeField] TMP_Text stageLevelText;
    [SerializeField] GameObject infoBoard;
    [SerializeField] TMP_Text targetNumberText;
    [SerializeField] GameObject enterButton;
    [SerializeField] CanvasGroup blockClick;
    [SerializeField] RectTransform screenView;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject retryButton;

    [Header("References")]
    public Player player;
    public Enemy enemy;
    [SerializeField] CardSlotManager cardSlotManager;
    [SerializeField] CardBoardManager cardBoardManager;
    [SerializeField] CalculationSequenceManager calculationSequenceManager;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] ExpressionManager expressionManager;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        Time.timeScale = TimeScale;
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        // 초기화
        StageLevel = 1;
        scoreManager.Init();
        player.Atk = 0;
        player.Health = Life;
        enemy.Atk = 1;
        scoreText.text = "계산 히어로";

        // 준비 단계
        player.gameObject.SetActive(false);
        enemy.gameObject.SetActive(false);
        infoBoard.SetActive(false);
        targetNumberText.text = string.Empty;
        stageLevelText.text = string.Empty;
        enterButton.GetComponent<Button>().interactable = false;
        blockClick.blocksRaycasts = false;

        // 액션 뷰
        screenView.offsetMin = new Vector2(0f, -600f);
        screenView.offsetMax = new Vector2(0f, 600f);

        UniTask.Void(async () =>
        {
            await UniTask.WaitForSeconds(1);

            if (StageLevel == 1)
            {
                // 플레이어 등장
                await player.SpawnAsync();

                // 적 등장
                enemy.Health = 100;
                await enemy.SpawnAsync();

                await UniTask.WaitForSeconds(2f);
            }

            // 플레이어가 죽을 때까지 스테이지 반복
            while (Life > 0)
            {
                IsInputExpressionComplete = false;
                targetNumberText.text = string.Empty;

                // 정리
                cardSlotManager.Clear();
                cardBoardManager.Clear();

                // 적이 죽어있으면
                if (enemy.Health <= 0)
                {
                    // 적 등장
                    enemy.Health = 100;
                    await enemy.SpawnAsync();

                    // 스테이지 넘어감
                    StageLevel++;
                }

                await UniTask.WaitForSeconds(0.5f);

                // 스테이지 정보
                stageLevelText.text = $"Stage {StageLevel}";
                infoBoard.SetActive(true);

                // 카드 깔기
                cardBoardManager.SetupCards();

                // 랜덤 숫자 지정
                TargetNumber = Random.Range(50, 151);
                targetNumberText.text = $"{TargetNumber} 만들기";

                // 버튼 활성화
                enterButton.GetComponent<Button>().interactable = false;
                enterButton.SetActive(true);

                // UI 올리기
                await screenView.DOAnchorPosY(600f, 1.5f).SetEase(Ease.OutQuart).ToUniTask();

                // 스코어 초기화
                scoreManager.Init();

                player.Atk = 0;

                blockClick.blocksRaycasts = true;

                // 플레이어 입력 판정이 들어올 때까지 대기
                await UniTask.WaitUntil(() => IsInputExpressionComplete);

                // 연출 동안 허튼 짓 못하게
                blockClick.blocksRaycasts = false;

                // 버튼 비활성화
                enterButton.SetActive(false);

                // 연출
                await calculationSequenceManager.PlayAsync();
            }

            // 게임 오버
            scoreText.text = "게임 오버";
            blockClick.blocksRaycasts = true;
            retryButton.gameObject.SetActive(true);

            await UniTask.CompletedTask;
        });
    }

    public bool CheckAnswerCurrentExpression()
    {
        bool result = expressionManager.CheckAnswerCurrentExpression();

        enterButton.GetComponent<Button>().interactable = result;

        return result;
    }

    public void OnClickRetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
