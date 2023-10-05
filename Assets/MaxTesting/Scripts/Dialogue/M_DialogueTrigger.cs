using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")] [SerializeField] private GameObject visualCue;

    [Header("InkJson")] [SerializeField] private TextAsset inkJson;

    private bool _playerInRange;
    public bool PlayerInRange
    {
        set => _playerInRange = value;
    }

    private void Awake()
    {
        visualCue.SetActive(false);
        _playerInRange = false;
    }

    private void Update()
    {
        if (_playerInRange && !M_DialogueManager.GetInstance().dialogueIsPlaying)
        {
            visualCue.SetActive(_playerInRange);
            if (M_InputManager.GetInstance().GetInteractPressed())
            {
                M_DialogueManager.GetInstance().EnterDialogueMode(inkJson);
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
        
    }
}
