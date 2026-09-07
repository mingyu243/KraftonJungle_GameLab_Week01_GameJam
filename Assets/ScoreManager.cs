using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Transform scoreBonusPopParentTr;
    [SerializeField] ScoreBonusPop scoreBonusPopPrefab;
    [Space]
    [SerializeField] int numberCardBonus;
    [SerializeField] int operatorCardBonus;
    [Space]
    [SerializeField] int score;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText;

    public int Score 
    { 
        get => score;
        set
        {
            score = value;
            scoreText.text = $"{Score}";
        }
    }

    public void Init()
    {
        numberCardBonus = 0;
        operatorCardBonus = 1;
        Score = 0;
    }

    public void AddNumberCardBonus(int value, Transform bonusPopPoint)
    {
        numberCardBonus += value;

        ScoreBonusPop newScoreBonusPop = Instantiate(scoreBonusPopPrefab, scoreBonusPopParentTr);
        newScoreBonusPop.transform.position = bonusPopPoint.position;
        newScoreBonusPop.ShowAsync($"+{value}").Forget();

        UpdateScore();
    }
    public void MulOperatorCardBonus(int value, Transform bonusPopPoint)
    {
        operatorCardBonus *= value;

        ScoreBonusPop newScoreBonusPop = Instantiate(scoreBonusPopPrefab, scoreBonusPopParentTr);
        newScoreBonusPop.transform.position = bonusPopPoint.position;
        newScoreBonusPop.ShowAsync($"x{value}").Forget();

        UpdateScore();
    }

    public void UpdateScore()
    {
        Score = numberCardBonus * operatorCardBonus;
        scoreText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, vibrato: 1, elasticity: 1f);
    }
}
