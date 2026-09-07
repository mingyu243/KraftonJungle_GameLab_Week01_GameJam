using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Transform scoreBonusPopParentTr;
    [SerializeField] ScoreBonusPop scoreBonusPopPrefab;
    [Space]
    [SerializeField] int sumBonus;
    [SerializeField] int mulBonus;
    [Space]
    [SerializeField] int score;

    [Header("UI")]
    [SerializeField] TMP_Text scoreBonusText;

    public int Score 
    { 
        get => score;
    }

    public void Init()
    {
        sumBonus = 0;
        mulBonus = 1;
        score = 0;
        scoreBonusText.text = $"{sumBonus} x {mulBonus}";
    }

    public void AddSumBonus(int value, Transform bonusPopPoint)
    {
        sumBonus += value;

        ScoreBonusPop newScoreBonusPop = Instantiate(scoreBonusPopPrefab, scoreBonusPopParentTr);
        newScoreBonusPop.transform.position = bonusPopPoint.position;
        newScoreBonusPop.ShowAsync($"left\n+{value}").Forget();

        UpdateScore();
    }
    public void AddMulBonus(int value, Transform bonusPopPoint)
    {
        mulBonus += value;

        ScoreBonusPop newScoreBonusPop = Instantiate(scoreBonusPopPrefab, scoreBonusPopParentTr);
        newScoreBonusPop.transform.position = bonusPopPoint.position;
        newScoreBonusPop.ShowAsync($"right\n+{value}").Forget();

        UpdateScore();
    }
    public void MulMulBonus(float value, Transform bonusPopPoint)
    {
        mulBonus = Mathf.RoundToInt(mulBonus * value);

        ScoreBonusPop newScoreBonusPop = Instantiate(scoreBonusPopPrefab, scoreBonusPopParentTr);
        newScoreBonusPop.transform.position = bonusPopPoint.position;
        newScoreBonusPop.ShowAsync($"right\nx{value}").Forget();

        UpdateScore();
    }

    public void UpdateScore()
    {
        score = sumBonus * mulBonus;
        scoreBonusText.text = $"{sumBonus} x {mulBonus}";
        scoreBonusText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, vibrato: 1, elasticity: 1f);
    }
}
