using Random = UnityEngine.Random;
using UnityEngine.Events;
using UnityEngine;
using System.Collections;
using System;

public class RotationHandler : MonoBehaviour
{
    // Minimum and maximum values for the Y rotation of the object
    [SerializeField, Range(0, 10)] private float tolerance;
    [SerializeField] private bool isTargetZero;
    [SerializeField] private Transform targetTransform;

    // Values for lerp
    [SerializeField] private float waitingTime = 3;

    // Event that is invoked when the object rotations is within the desired range
    public UnityEvent RotationMatched = new UnityEvent();
    public UnityEvent NewRotation = new UnityEvent();

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
        NewRotation.Invoke();
    }

    // Function that fires after the object is rotated
    public void OnRotated()
    {
        Vector3 currentForward = transform.forward;
        Vector3 targetForward = targetTransform.forward;
        float accuracy = Vector3.Dot(currentForward, targetForward);
        if (accuracy >= 20 - tolerance)
        {
            RotationMatched.Invoke();
        }

        print("rotated");
    }

    // Function that fires after the hologram matched the desired rotation range
    private void OnMatched()
    {
        transform.rotation = targetTransform.rotation;
        StartCoroutine(DelayedTask(waitingTime, RotateAnimation));
        print("matched");
    }

    IEnumerator DelayedTask(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
