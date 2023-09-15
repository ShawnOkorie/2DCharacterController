using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
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
        visualCue.SetActive(_playerInRange);
        if (Max_InputManager.GetInstance().GetInteractPressed() && _playerInRange)
        {
            Debug.Log(inkJson.text);
        }
    }
}
