using System.Collections.Generic;
using UnityEngine;

public class CardBoardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CardSpawner cardSpawner;

    public void SetupCards()
    {
        cardSpawner.SetupCards();
    }
}