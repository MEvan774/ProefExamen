using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    [Range(0, 30)]
    [SerializeField] private float dropDownLength = 3.32f;

    [SerializeField] private AnimationCurve easeCurve;

    private AudioSource _notifyAudio;
    private GameObject _dropDownMenu;
    private Vector2 _transRef;
    private Text _dropDownText;

    // Start is called before the first frame update
    void Start()
    {
        _dropDownText = GetComponentInChildren<Text>();
        _dropDownMenu = gameObject;
        _transRef = new Vector2(_dropDownMenu.transform.localPosition.x, _dropDownMenu.transform.localPosition.y);

        _notifyAudio = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            DropEaseEvent("DropDownTextInput");
        }
    }

    void DropEaseEvent(string _TextInput)
    {
        _dropDownText.text = _TextInput;
        StartCoroutine(DropEase());
    }

    public IEnumerator DropEase()
    {
        _notifyAudio.Play();

        Handheld.Vibrate();

        float elapsedTime = 0;


        while (elapsedTime <= 1f)
        {
            elapsedTime += Time.deltaTime;

            float ease = Mathf.Lerp(0, 1, elapsedTime);

            _dropDownMenu.transform.localPosition = new Vector2(_dropDownMenu.transform.localPosition.x, _dropDownMenu.transform.localPosition.y + EaseInOutBack(ease) * -dropDownLength);

            yield return null;
        }

        yield return new WaitForSeconds(2);

        elapsedTime = 0;

        while (elapsedTime <= 1f)
        {
            elapsedTime += Time.deltaTime;

            float ease = Mathf.Lerp(0, 1, elapsedTime);

            _dropDownMenu.transform.localPosition = new Vector2(_dropDownMenu.transform.localPosition.x, _dropDownMenu.transform.localPosition.y + EaseInOutBack(ease) * dropDownLength);

            yield return null;
        }

        _dropDownMenu.transform.localPosition = _transRef;
    }

    float EaseInOutBack(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return x < 0.5
        ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
        : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }
}
