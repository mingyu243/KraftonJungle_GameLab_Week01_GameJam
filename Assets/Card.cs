using System;
using TMPro;
using UnityEngine;

public enum CardCategory
{
    None,
    Number,
    Operator
}

public enum CardType
{
    None = 0,
    Number1 = 1,
    Number2 = 2,
    Number3 = 3,
    Number4 = 4,
    Number5 = 5,
    Number6 = 6,
    Number7 = 7,
    Number8 = 8,
    Number9 = 9,
    Plus = 10,
    Minus = 11,
    Multiply = 12,
}

[Serializable]
public class CardData
{
    public CardCategory CardCategory;
    public CardType CardType;
}

public class Card : MonoBehaviour
{
    public CardData CardData;
    [Space]
    [SerializeField] TMP_Text text;

    public void UpdateUI()
    {
        string str = string.Empty;

        switch (CardData.CardType)
        {
            case CardType.None: break;
            case CardType.Number1: str = "1"; break;
            case CardType.Number2: str = "2"; break;
            case CardType.Number3: str = "3"; break;
            case CardType.Number4: str = "4"; break;
            case CardType.Number5: str = "5"; break;
            case CardType.Number6: str = "6"; break;
            case CardType.Number7: str = "7"; break;
            case CardType.Number8: str = "8"; break;
            case CardType.Number9: str = "9"; break;
            case CardType.Plus: str = "+"; break;
            case CardType.Minus: str = "-"; break;
            case CardType.Multiply: str = "x"; break;
            default:
                break;
        }

        text.text = str;
    }
}
