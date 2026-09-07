using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class ExpressionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CardSlotManager cardSlotManager;
    List<CardSlot> CardSlotList => cardSlotManager.CardSlotList;

    DataTable dataTable = new DataTable();

    // 수식이 계산 가능한지
    public bool IsValidExpression()
    {
        string expression = GetExpression();

        if (string.IsNullOrWhiteSpace(expression))
        {
            Debug.Log($"공백이라 계산 X / {expression}");
            return false;
        }

        // 첫번째에 숫자가 오는 지
        if (!Regex.IsMatch(expression, @"^[ ]*[0-9]"))
        {
            Debug.Log($"첫번째에 숫자가 아니라서 계산 X / {expression}");
            return false;
        }

        // 연산자가 2개 이상 연결되어 있으면 안 됨
        if (Regex.IsMatch(expression, @"[\+\-\*/]\s*[\+\-\*/]"))
        {
            Debug.Log($"연산자가 2개 이상 연결되어있어서 계산 X / {expression}");
            return false;
        }

        try
        {
            int result = Convert.ToInt32(dataTable.Compute(expression, string.Empty));

            Debug.Log($"계산 가능 / {expression} / result = {result}");

            return true;
        }
        catch
        {
            Debug.Log($"계산 불가 / {expression}");
            return false;
        }
    }

    public bool CheckAnswerCurrentExpression()
    {
        bool isValid = IsValidExpression();

        if (isValid)
        {
            int result = GetExpressionResult();
            return CheckAnswer(result);
        }

        return false;
    }

    // 수식이 정답인지
    public bool CheckAnswer(int answer)
    {
        return (answer == GameManager.Instance.TargetNumber);
    }

    // 수식 결과
    public int GetExpressionResult()
    {
        string expression = GetExpression();

        int result = Convert.ToInt32(dataTable.Compute(expression, string.Empty));

        return result;
    }

    string GetExpression()
    {
        StringBuilder expressionStr = new StringBuilder();

        foreach (CardSlot slot in CardSlotList)
        {
            if (slot.Card != null)
            {
                string s = string.Empty;
                switch (slot.Card.CardData.CardType)
                {
                    case CardType.None: break;
                    case CardType.Number1: s = "1"; break;
                    case CardType.Number2: s = "2"; break;
                    case CardType.Number3: s = "3"; break;
                    case CardType.Number4: s = "4"; break;
                    case CardType.Number5: s = "5"; break;
                    case CardType.Number6: s = "6"; break;
                    case CardType.Number7: s = "7"; break;
                    case CardType.Number8: s = "8"; break;
                    case CardType.Number9: s = "9"; break;
                    case CardType.Plus: s = "+"; break;
                    case CardType.Minus: s = "-"; break;
                    case CardType.Multiply: s = "*"; break;
                    default:
                        break;
                }
                expressionStr.Append(s);
            }
        }

        return expressionStr.ToString();
    }
}
