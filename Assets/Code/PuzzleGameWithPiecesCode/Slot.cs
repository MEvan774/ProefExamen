using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    [SerializeField] private PuzzleGameManager _manager;
    [SerializeField] private int _slotID;

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            PuzzlePiece currentPiece = eventData.pointerDrag.GetComponent<PuzzlePiece>();

            if (_slotID == currentPiece.dragID && !currentPiece.isCorrectPosition)
            {
                currentPiece.isCorrectPosition = true;
                _manager.OnCorrectPlaced();
            }
            else if(_slotID != currentPiece.dragID && currentPiece.isCorrectPosition)
            {
                currentPiece.isCorrectPosition = false;
                _manager.correctplaced--;
            }

        }
    } 
}
