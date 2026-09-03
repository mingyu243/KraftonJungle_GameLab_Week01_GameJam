using System;
using UnityEngine;

public enum CardType
{
    None,
    Number,
    Operator
}

public enum OperatorType
{
    None,
    Plus,
    Minus,
    Multiply
}

[Serializable]
public class CardData
{
    public CardType CardType;

    public int Number;
    public OperatorType OperatorType;
}

public class Card : MonoBehaviour
{
    public CardData CardData;
}
