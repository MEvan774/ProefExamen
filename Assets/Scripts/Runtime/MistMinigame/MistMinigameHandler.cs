using UnityEngine;
using UnityEngine.Events;

public class MistMinigameHandler : MonoBehaviour
{
    [SerializeField] private AudioPitch_Player1 _audioInput;
    [SerializeField] private MicrophoneCapture _micInput;
    [SerializeField] private Transform _camTrans;

    [SerializeField] private UnityEvent _onMistGameCompleted;

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
        _pitchInput = _micInput.loudness;
 
        if (_pitchInput > 0.006f)//removes mist when mic input treshold has passed
        {
            _mistAmount -= (Time.deltaTime / 6);
            _mistRenderer.material.SetFloat("_Alpha", _mistAmount);
        }
        else if (_mistAmount < 1 && _mistAmount > 0)//when no input is detected, the mist will come back
        {
            _mistAmount += (Time.deltaTime / 10);
            _mistRenderer.material.SetFloat("_Alpha", _mistAmount);
        }

        RaycastHit _hit;

        if(Physics.Raycast(_camTrans.position, transform.TransformDirection(Vector3.forward), out _hit, 200))
        {
            _isLookingAt = true;
        }
        else
            _isLookingAt = false;


        if (_mistAmount < 0.15f && _isLookingAt && !_isGameClear)//when player found the fox, the scene will transition to main scene
        {
            _isGameClear = true;
            OnMinigameClear();
        }
    }

    void OnMinigameClear()//when minigame is completed, this event will be invoked
    {
        _onMistGameCompleted.Invoke();
    }

    private void OnTriggerEnter(Collider other)//checks if player looks at fox
    {
        _isLookingAt = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _isLookingAt = false;
    }
}
