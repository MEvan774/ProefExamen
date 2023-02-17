using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropHandler : MonoBehaviour
{
    private GameObject _selectedObj;

    void Start()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_selectedObj != null)
            {
                RaycastHit hit = castRay();

                if(hit.collider != null)
                {
                    if (!hit.collider.CompareTag("drag"))
                    {
                        return;
                    }

                    _selectedObj = hit.collider.gameObject;
                    Cursor.visible = false;
                }
            }
            else
            {
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(_selectedObj.transform.position).z);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(position);
                _selectedObj.transform.position = new Vector3(worldPos.x, worldPos.y, 0);

                _selectedObj = null;
                Cursor.visible = true;
            }

            if (_selectedObj != null)
            {
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(_selectedObj.transform.position).z);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(position);
                _selectedObj.transform.position = new Vector3(worldPos.x, worldPos.y, 0.25f);
            }
        }
    }

    private RaycastHit castRay()
    {
        Vector3 screenToMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 screenToMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenToMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenToMousePosNear);

        RaycastHit hit;

        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);

        return hit;
    }
}
