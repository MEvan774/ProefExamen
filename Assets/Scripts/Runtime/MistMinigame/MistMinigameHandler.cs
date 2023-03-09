using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MistMinigameHandler : MonoBehaviour
{
    [SerializeField] private AudioPitch_Player1 _audioInput;
    [SerializeField] private UnityEvent onMistGameCompleted;

    private MeshRenderer _mistRenderer;

    private float _pitchInput;
    private float _mistAmount = 1;

    private bool _isGameClear = false;
    private bool _isLookingAt = false;

    void Start()
    {
        _mistRenderer = GetComponent<MeshRenderer>();
    }

    void LateUpdate()
    {
        _pitchInput = _audioInput.CurrentPublicPitch;

        if (_pitchInput > 5)
        {
            _mistAmount -= (Time.deltaTime / 10);
            _mistRenderer.material.SetFloat("_Alpha", _mistAmount);
        }
        if (_mistAmount < 0.05f && _isLookingAt && !_isGameClear)
        {
            _isGameClear = true;
            OnMinigameClear();
        }
    }

    void OnMinigameClear()
    {
        onMistGameCompleted.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        _isLookingAt = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _isLookingAt = false;
    }
}
