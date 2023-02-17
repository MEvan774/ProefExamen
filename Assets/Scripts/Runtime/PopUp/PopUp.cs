using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    GameObject dropDownMenu;

    [Range(0, 30)]
    [SerializeField] float dropDownLength = 3.32f;

    [SerializeField] AnimationCurve easeCurve;

    private AudioSource notifyAudio;
    private Vector2 transRef;
    private Text dropDownText;

    // Start is called before the first frame update
    void Start()
    {
        dropDownText = GetComponentInChildren<Text>();
        dropDownMenu = gameObject;
        transRef = new Vector2(dropDownMenu.transform.localPosition.x, dropDownMenu.transform.localPosition.y);

        notifyAudio = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            DropEaseEvent("dlof");
        }
    }

    void DropEaseEvent(string _TextInput)
    {
        dropDownText.text = _TextInput;
        StartCoroutine(dropEase());
    }

    public IEnumerator dropEase()
    {
        notifyAudio.Play();

        Handheld.Vibrate();

        float elapsedTime = 0;


        while (elapsedTime <= 1f)
        {
            elapsedTime += Time.deltaTime;

            float ease = Mathf.Lerp(0, 1, elapsedTime);

            dropDownMenu.transform.localPosition = new Vector2(dropDownMenu.transform.localPosition.x, dropDownMenu.transform.localPosition.y + easeInOutBack(ease) * -dropDownLength);

            yield return null;
        }

        yield return new WaitForSeconds(2);

        elapsedTime = 0;

        while (elapsedTime <= 1f)
        {
            elapsedTime += Time.deltaTime;

            float ease = Mathf.Lerp(0, 1, elapsedTime);

            dropDownMenu.transform.localPosition = new Vector2(dropDownMenu.transform.localPosition.x, dropDownMenu.transform.localPosition.y + easeInOutBack(ease) * dropDownLength);

            yield return null;
        }


        dropDownMenu.transform.localPosition = transRef;


    }

    float easeInOutBack(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return x < 0.5
        ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
        : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }
}
