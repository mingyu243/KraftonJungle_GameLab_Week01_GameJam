using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    public int Life = 3;
    [Range(0, 3)] public int TimeScale = 1;
    public int ScoreFullSlotAddMulBonus = 50;
    public int ScorePlusMinusAddMulBonus = 2;
    public int ScoreMulMulMulBonus = 2;
    public int ScorePairMulMulBonus = 2;
    public int ScoreTripleMulMulBonus = 5;

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
    [SerializeField] TMP_Text scoreBonusText;
    [SerializeField] GameObject retryButton;
    [SerializeField] GameObject helpButton;

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
        scoreBonusText.text = "계산 히어로";

        // 준비 단계
        player.gameObject.SetActive(false);
        enemy.gameObject.SetActive(false);
        infoBoard.SetActive(false);
        targetNumberText.text = string.Empty;
        stageLevelText.text = string.Empty;
        enterButton.GetComponent<Button>().interactable = false;
        blockClick.blocksRaycasts = false;
        helpButton.SetActive(false);

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
                enemy.Health = GetRandomEnemyHP(StageLevel);
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
                    // 스테이지 넘어감
                    StageLevel++;

                    // 적 등장
                    enemy.Health = GetRandomEnemyHP(StageLevel);
                    await enemy.SpawnAsync();
                }

                await UniTask.WaitForSeconds(0.5f);

                // 스테이지 정보
                stageLevelText.text = $"Stage {StageLevel}";
                infoBoard.SetActive(true);

                // 도움말 버튼
                helpButton.SetActive(true);

                // 카드 깔기
                cardBoardManager.SetupCards();

                // 랜덤 숫자 지정
                TargetNumber = GetRandomTargetNumber(StageLevel);
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
            scoreBonusText.text = "게임 오버";
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

    int GetRandomTargetNumber(int stage)
    {
        float baseTarget = 30 + (stage * 6);
        int target = Mathf.RoundToInt(baseTarget + Random.Range(-5f, 5f));
        return Mathf.Clamp(target, 20, 120);
    }

    int GetRandomEnemyHP(int stage)
    {
        int targetHP = 0;

        if (stage == 1)
        {
            targetHP = 20;
        }
        else if (stage <= 2)
        {
            // 의도: 평균보다 못해도(대충 쳐도) 무조건 한 방에 잡히는 넉넉한 샌드박스 구간
            targetHP = Mathf.RoundToInt(60f * Mathf.Pow(1.3f, stage - 1));
        }
        else if (stage <= 10)
        {
            // 의도: 몬스터 체력이 점차 올라가며, '딱 평균 정도'의 수식을 완성해야 한 방에 잡히는 구간
            float baseHP = 150f;
            targetHP = Mathf.RoundToInt(baseHP * Mathf.Pow(1.35f, stage - 1));
        }
        else
        {
            // 의도: 몬스터 체력이 확 뛰는 게 아니라 완만하게 오르므로, 
            // 플레이어가 평균보다 조금 더 잘 쳐주거나(중고점) 빌드를 갖추면 계속 밀고 나갈 수 있는 구간
            int extraStage = stage - 15;
            int baseAt15 = Mathf.RoundToInt(150f * Mathf.Pow(1.35f, 14));

            targetHP = baseAt15 + (extraStage * 12000);
        }

        return targetHP;
    }
}
