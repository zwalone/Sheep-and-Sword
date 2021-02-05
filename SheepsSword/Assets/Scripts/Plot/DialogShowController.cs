using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogShowController : MonoBehaviour
{
    // Text:
    public Text textDisplay;
    public string[] sentences;
    private int index;
    public float typingSpeed;

    // Event:
    private GameObject dialog;
    private bool isDisplayed = false;
    private PlayerController playerInfo;
    private Coroutine typing;

    private void Awake()
    {
        dialog = GameObject.Find("UI").transform.Find("Dialog").gameObject;
        playerInfo = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Time.timeScale != 1) return;

        if (isDisplayed)
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                NextSentence();
    }

    public void StartDialog()
    {
        textDisplay.text = "";
        isDisplayed = true;
        dialog.SetActive(true);
        typing = StartCoroutine(Type());
    }

    public IEnumerator Type()
    {
        gameObject.GetComponent<SoundController>().PlaySound(index);
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextSentence() 
    {
        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StopCoroutine(typing);
            typing = StartCoroutine(Type());
        }
        else
        {
            dialog.SetActive(false);
            textDisplay.text = "";
            isDisplayed = false;
            playerInfo.StopReading();
            gameObject.GetComponent<AudioSource>().Stop();
        }
    }
}
