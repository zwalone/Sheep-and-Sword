using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public Text textDisplay;
    public string[] sentences;
    private int index;
    public float typingSpeed;
    private GameObject dialog;

    void Start() 
    { 
        dialog = GameObject.Find("Dialog"); 
        StartCoroutine(Type());  
    }

    IEnumerator Type()
    {
        dialog.GetComponent<AudioSource>().Play();
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        dialog.GetComponent<AudioSource>().Stop();
    }

    public void NextSentence() 
    {
        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
        {
            dialog.SetActive(false);
            textDisplay.text = "";
        }
    }
}
