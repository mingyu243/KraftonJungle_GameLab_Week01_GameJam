using System.Collections.Generic;
using UnityEngine;

public class CardBoardManager : MonoBehaviour
{
    [SerializeField] CardSpawner cardSpawner;

    public void SetupCards()
    {
        cardSpawner.SetupCards();
    }
}