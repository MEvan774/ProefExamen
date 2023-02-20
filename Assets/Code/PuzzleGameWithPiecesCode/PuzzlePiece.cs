using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public int dragID;

    Vector2 lastTouchPos;

    private float _x, _y, _z;
    private CanvasGroup _canvasGroup;
    [HideInInspector] public bool isCorrectPosition = false;

    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastTouchPos = eventData.position;
        _canvasGroup.blocksRaycasts = false;

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentTouchPos = eventData.position;
        Vector2 diff = currentTouchPos - lastTouchPos;

        RectTransform rectTrans = GetComponent<RectTransform>();

        _x = diff.x;
        _y = diff.y;
        _z = transform.localPosition.z;

        rectTrans.position = rectTrans.position + new Vector3(_x, _y, _z);

        lastTouchPos = currentTouchPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
    }
}