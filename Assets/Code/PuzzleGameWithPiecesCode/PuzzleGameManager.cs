using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGameManager : MonoBehaviour
{
    [HideInInspector] public int CorrectplacedPieces;

    public void OnCorrectPlaced()
    {
        CorrectplacedPieces++;

        if(CorrectplacedPieces >= 9)
            OnPuzzleCompleted();
    }

    void OnPuzzleCompleted()
    {
        //Dispatches event when puzzle is completed
    }
}
