﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LastFightDialogShowController : MonoBehaviour
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
    private AudioSource[] sounds;

    private void Awake()
    {
        dialog = GameObject.Find("UI").transform.Find("Dialog").gameObject;
        playerInfo = GameObject.Find("Player").GetComponent<PlayerController>();
        sounds = GameObject.Find("Music").GetComponents<AudioSource>();
    }

    private void Update()
    {
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
        dialog.GetComponent<AudioSource>().Play();
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        dialog.GetComponent<AudioSource>().Stop();

        if (index == 2)
        {
            sounds[1].Play();
            yield return new WaitForSeconds(1.0f);
            sounds[2].Play();
        }
    }

    public void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            if (sentences[index] == "")
            {
                textDisplay.text += " ";
                index++;
            }
            if (sentences[index - 1] != "") textDisplay.text = "";
            StopCoroutine(typing);
            typing = StartCoroutine(Type());
        }
        else
        {
            dialog.SetActive(false);
            textDisplay.text = "";
            isDisplayed = false;
            playerInfo.StopReading();
            sounds[0].Play();
        }
    }
}