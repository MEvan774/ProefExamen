using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    [SerializeField] private int slotID;
    [SerializeField] private PuzzleGameManager manager;

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            PuzzlePiece currentPiece = eventData.pointerDrag.GetComponent<PuzzlePiece>();

            if (slotID == currentPiece.dragID && !currentPiece.IsCorrectPosition)
            {
                currentPiece.IsCorrectPosition = true;
                manager.OnCorrectPlaced();
            }
            else if(slotID != currentPiece.dragID && currentPiece.IsCorrectPosition)
            {
                currentPiece.IsCorrectPosition = false;
                manager.correctPlacedPieces--;
            }

        }
    } 
}
