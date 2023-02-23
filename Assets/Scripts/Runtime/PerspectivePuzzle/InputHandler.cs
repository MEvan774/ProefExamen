using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private RotationHandler rotationHandler;

    private readonly float _pcRotationSpeed = 10f;
    private readonly float _mobileRotationSpeed = 0.5f;
    private readonly float _maxDistance = 10f;
    private readonly float _zAngle = 0f;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void OnMouseDrag()
    {
        float xRotation = Input.GetAxis("Mouse X") * _pcRotationSpeed;
        float yRotation = Input.GetAxis("Mouse Y") * _pcRotationSpeed;

        Vector3 right = Vector3.Cross(cam.transform.up, transform.position - cam.transform.position);
        Vector3 up = Vector3.Cross(transform.position - cam.transform.position, right);

        transform.rotation = Quaternion.AngleAxis(-xRotation, up) * transform.rotation;
        transform.rotation = Quaternion.AngleAxis(yRotation, right) * transform.rotation;

        rotationHandler.OnRotated();
    }

    private void Update()
    {
        foreach (Touch touch in Input.touches) 
        {
            Ray ray = cam.ScreenPointToRay(touch.position);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, _maxDistance))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("TouchPhase.Began");
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Debug.Log("TouchPhase.Moved");
                    transform.Rotate(touch.deltaPosition.y * _mobileRotationSpeed, -touch.deltaPosition.x * _mobileRotationSpeed, _zAngle, Space.World);
                    rotationHandler.OnRotated();
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    Debug.Log("TouchPhase.Ended");
                }
            }
        }
    }
}
