using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGameManager : MonoBehaviour
{
    [HideInInspector] public int correctplaced;

    public void OnCorrectPlaced()
    {
        correctplaced++;

        if(correctplaced >= 9)
            OnPuzzleCompleted();
    }

    void OnPuzzleCompleted()
    {
        Debug.Log("PuzzleCompleted!");// Dispatch event
    }
}
