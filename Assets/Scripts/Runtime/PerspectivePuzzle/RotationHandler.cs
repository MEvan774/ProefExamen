using Random = UnityEngine.Random;
using UnityEngine.Events;
using UnityEngine;
using System.Collections;
using System;

public class RotationHandler : MonoBehaviour
{
    // Minimum and maximum values for the Y rotation of the object
    [SerializeField, Range(0f, 0.01f)] private float tolerance;
    [SerializeField] private bool isTargetZero;
    [SerializeField] private Transform targetTransform;

    [SerializeField] private GameObject puzzleComplete;
    [SerializeField] private GameObject puzzleInComplete;

    // Values for lerp
    [SerializeField] private float waitingTime = 3;

    // Event that is invoked when the object rotations is within the desired range
    public UnityEvent RotationMatched = new UnityEvent();

    private bool _isFinished = false;

    private void Start()
    {
        RotationMatched.AddListener(OnMatched);
        RotateAnimation();
    }

    // Function that Rotates the object to a random value
    public void RotateAnimation()
    {
        float targetY = targetTransform.rotation.eulerAngles.y;
        float rnd = Random.Range(targetY + 20, targetY + 340);
        StartCoroutine(Lerp(rnd, waitingTime));
    }

    IEnumerator Lerp(float endValue, float lerpDuration)
    {
        float startValue = transform.rotation.eulerAngles.y;
        float timeElapsed = 0;
        float valueToLerp;
        while (timeElapsed < lerpDuration)
        {
            valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, valueToLerp, 0);
            yield return null;
        }
        valueToLerp = endValue;
        transform.rotation = Quaternion.Euler(0, valueToLerp, 0);
    }

    // Function that fires after the object is rotated
    public void OnRotated()
    {
        Vector3 currentForward = transform.forward;
        Vector3 targetForward = targetTransform.forward;
        float accuracy = Vector3.Dot(currentForward, targetForward);
        if (accuracy >= 1f - tolerance)
        {
            transform.rotation = targetTransform.rotation;
            StartCoroutine(WaitForCompletion());
        }
    }

    IEnumerator WaitForCompletion()
    {
        yield return new WaitForSeconds(2f);
        RotationMatched.Invoke();
    }

    // Function that fires after the hologram matched the desired rotation range
    private void OnMatched()
    {
        if (!_isFinished)
        {
            transform.rotation = targetTransform.rotation;
            puzzleInComplete.SetActive(false);

            puzzleComplete.SetActive(true);
            print("matched");

            _isFinished = true;
        }
    }

    /*public GameObject[] puzzlePieces;
    public Camera mainCamera;
    public float distanceThreshold = 10.0f;

    public Transform cameraTransform;
    public Transform targetTransform;
    public float angleThreshold = 5.0f;
    public float positionThreshold = 1.0f;

    public Transform targetTransform1;
    public float angle = 45.0f;
    public float distance = 5.0f;

    private void Start()
    {
        // Calculate the position and orientation of the camera
        Vector3 position = targetTransform1.position - Quaternion.Euler(angle, 0, 0) * Vector3.forward * distance;
        Quaternion rotation = Quaternion.LookRotation(targetTransform1.position - position, Vector3.up);

        // Set the position and orientation of the camera
        transform.position = position;
        transform.rotation = rotation;
    }

    private void Update()
    {
        // Calculate the corners of the camera's frustum
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        // Check each puzzle piece for alignment with the camera view
        bool allAligned = true;
        foreach (GameObject puzzlePiece in puzzlePieces)
        {
            // Cast a ray from the camera to the puzzle piece
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, puzzlePiece.transform.position - mainCamera.transform.position, out hit))
            {
                // Check if the puzzle piece is within the camera view
                if (GeometryUtility.TestPlanesAABB(planes, puzzlePiece.GetComponent<Renderer>().bounds))
                {
                    // Check if the distance between the camera and the puzzle piece is within the threshold
                    if (hit.distance < distanceThreshold)
                    {
                        // The puzzle piece is aligned with the camera view
                    }
                    else
                    {
                        allAligned = false;
                        break;
                    }
                }
                else
                {
                    allAligned = false;
                    break;
                }
            }
        }

        // If all the puzzle pieces are aligned with the camera view, the puzzle is solved
        if (allAligned)
        {
            Debug.Log("Puzzle solved!");
        }
    }*/

    /*public Camera cam;
    public Transform centerTransform;
    public int numPoints = 8;
    public float radius = 5.0f;

    private Vector3[] points;

    private void Start()
    {
        // Calculate the positions of the points around the circle
        points = new Vector3[numPoints];
        float angle = 360.0f / numPoints;
        for (int i = 0; i < numPoints; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle * i) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle * i) * radius;
            points[i] = centerTransform.position + new Vector3(x, 0, y);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the circle and points in the Scene view
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(centerTransform.position, radius);
        Gizmos.color = Color.red;
        for (int i = 0; i < numPoints; i++)
        {
            Gizmos.DrawWireSphere(points[i], 0.5f);
        }
    }

    public bool IsGameObjectInPoint(GameObject targetGameObject)
    {
        // Check if the target game object is within any of the points
        for (int i = 0; i < numPoints; i++)
        {
            if (Vector3.Distance(targetGameObject.transform.position, points[i]) < 0.5f * radius)
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        //print(IsGameObjectInPoint(cam));
    }*/
}