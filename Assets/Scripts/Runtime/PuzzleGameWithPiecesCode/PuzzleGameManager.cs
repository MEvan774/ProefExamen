using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleGameManager : MonoBehaviour
{
    [HideInInspector] public int correctPlacedPieces;
    [SerializeField] private UnityEvent onPuzzleCompleted;
    public void OnCorrectPlaced()
    {
        correctPlacedPieces++;

        if(correctPlacedPieces >= 9)
            OnPuzzleCompleted();
    }

    void OnPuzzleCompleted()
    {
        onPuzzleCompleted.Invoke();
    }
}
