using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputFunctions : MonoBehaviour
{
    private void Update()
    {
        object GetButtonUp(ref InputAction inputAction)
        {
            var inputActionActiveControl = inputAction.activeControl as ButtonControl;
            if (inputActionActiveControl != null)
            {
                print("true");
                return inputActionActiveControl.wasReleasedThisFrame;
            }
            print("false");
            return false;
        }
    }
}
