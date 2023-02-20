using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public int dragID;
    [HideInInspector] public bool IsCorrectPosition = false;

    private float _x, _y, _z;
    private Vector2 _lastTouchPos;
    private CanvasGroup _canvasGroup;

    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastTouchPos = eventData.position;
        _canvasGroup.blocksRaycasts = false;

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentTouchPos = eventData.position;
        Vector2 diff = currentTouchPos - _lastTouchPos;

        RectTransform rectTrans = GetComponent<RectTransform>();

        _x = diff.x;
        _y = diff.y;
        _z = transform.localPosition.z;

        rectTrans.position = rectTrans.position + new Vector3(_x, _y, _z);

        _lastTouchPos = currentTouchPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
    }
}