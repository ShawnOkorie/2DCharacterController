using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;

public class M_DialogueManager : MonoBehaviour
{

    [Header("Dialogue UI")] 
    
    [SerializeField] private GameObject dialoguePanel;

    [SerializeField] private TextMeshProUGUI dialogueText;

    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }
    
    private static M_DialogueManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager");
        }
        instance = this;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }

    public static M_DialogueManager GetInstance()
    {
        return instance;
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if (M_InputManager.GetInstance().GetSubmitPressed())
        {
            ContinueStory();
        }
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
}
