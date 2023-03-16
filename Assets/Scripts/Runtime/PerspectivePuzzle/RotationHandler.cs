using Random = UnityEngine.Random;
using UnityEngine.Events;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class RotationHandler : MonoBehaviour
{
    // Minimum and maximum values for the rotation of the object
    [SerializeField, Range(0f, 0.01f)] private float tolerance;
    [SerializeField] private bool isTargetZero;
    [SerializeField] private Transform targetTransform;

    [SerializeField] private CollisionCheck collisionCheck;

    // Values for lerp
    [SerializeField] private float waitingTime = 1f;

    // Event that is invoked when the object rotations is within the desired range
    public UnityEvent RotationMatched = new UnityEvent();

    private static readonly int InPlace = Animator.StringToHash("InPlace");
    private bool _isFinished = false;

    private void Start()
    {
        RotateAnimation();
    }

    // Function that Rotates the object to a random value
    public void RotateAnimation()
    {
        float targetY = targetTransform.rotation.eulerAngles.magnitude;
        float rnd = Random.Range(targetY + 20, targetY + 340);
        StartCoroutine(Lerp(rnd, waitingTime));
    }

    IEnumerator Lerp(float endValue, float lerpDuration)
    {
        float startValue = transform.rotation.eulerAngles.magnitude;
        float timeElapsed = 0;
        float valueToLerp;
        while (timeElapsed < lerpDuration)
        {
            valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            transform.rotation = Quaternion.Euler(valueToLerp, valueToLerp, valueToLerp);
            yield return null;
        }
        valueToLerp = endValue;
        transform.rotation = Quaternion.Euler(valueToLerp, valueToLerp, valueToLerp);
    }

    // Function that fires after the object is rotated
    public void OnRotated()
    {
        if (collisionCheck.InPosition)
        {
            gameObject.GetComponent<Animator>().SetBool(InPlace, true);
            StartCoroutine(WaitForCompletion());
        } else if (!collisionCheck.InPosition)
        {
            gameObject.GetComponent<Animator>().SetBool(InPlace, false);
        }
    }

    IEnumerator WaitForCompletion()
    {
        OnMatched();

        yield return new WaitForSeconds(2f);

        RotationMatched.Invoke();
    }

    // Function that fires after the hologram matched the desired rotation range
    private void OnMatched()
    {
        if (!_isFinished)
        {
            _isFinished = true;
        }
    }
}