using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] int numberCardBonus;
    [SerializeField] int plusMinusCardBonus;
    [SerializeField] int mulCardBonus;
    public int Score;

    [Header("UI")]
    [SerializeField] TMP_Text numberCardBonusText;
    [SerializeField] TMP_Text plusMinusCardBonusText;
    [SerializeField] TMP_Text mulCardBonusText;
    [SerializeField] TMP_Text scoreText;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        numberCardBonus = 0;
        plusMinusCardBonus = 1;
        mulCardBonus = 1;
        Score = 0;

        numberCardBonusText.text = $"{numberCardBonus}";
        plusMinusCardBonusText.text = $"{plusMinusCardBonus}";
        mulCardBonusText.text = $"{mulCardBonus}";
        scoreText.text = $"{Score}";
    }

    public void AddNumberCardBonus(int value)
    {
        numberCardBonus += value;
        numberCardBonusText.text = $"{numberCardBonus}";
        numberCardBonusText.transform.DOPunchScale(Vector3.one * 1.3f, 0.2f, vibrato: 1, elasticity: 1f);

        UpdateScore();
    }
    public void AddPlusMinusCardBonus(int value)
    {
        plusMinusCardBonus += value;
        plusMinusCardBonusText.text = $"{plusMinusCardBonus}";
        plusMinusCardBonusText.transform.DOPunchScale(Vector3.one * 1.3f, 0.2f, vibrato: 1, elasticity: 1f);

        UpdateScore();
    }
    public void MulMulCardBonus(int value)
    {
        mulCardBonus *= value;
        mulCardBonusText.text = $"{mulCardBonus}";
        mulCardBonusText.transform.DOPunchScale(Vector3.one * 1.3f, 0.2f, vibrato: 1, elasticity: 1f);

        UpdateScore();
    }

    public void UpdateScore()
    {
        Score = numberCardBonus * plusMinusCardBonus * mulCardBonus;
        scoreText.text = $"{Score}";
        scoreText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, vibrato: 1, elasticity: 1f);
    }
}
