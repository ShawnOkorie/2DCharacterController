using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConsoleController : MonoBehaviour
{
    [SerializeField] private PlayerManager player;

    public bool showConsole { private set; get; }
    private bool showHelp;
    private string input;
    private Vector2 scroll;
    private GUIStyle myStyle;

    #region ConsoleCommands
    public static ConsoleCommand HELP;
    public static ConsoleCommand<bool> WALLSTICK;
    public static ConsoleCommand<bool> WALLJUMP;
    public static ConsoleCommand<bool> WALLCLIMB;
    public static ConsoleCommand<bool> VARJUMP;
    public static ConsoleCommand<bool> DOUBLEJUMP;
    #endregion
    
    public List<object> commandList;

    public void OnToggleDebug()
    {
        showConsole = !showConsole;
        input = "";
    }
    public void OnReturn()
    {
        if (showConsole)                                                                            
        {
            HandleInput();
            input = "";
        }
    }
    private void Awake()
    {
        InitConsoleCommands();
        
        myStyle = new GUIStyle();
        myStyle.fontSize = 30;
        myStyle.normal.textColor = Color.white;
    }
    private void InitConsoleCommands()
    {
        HELP = new ConsoleCommand("help", "Shows a List of all available Commads", "help",
            () =>
            {
                showHelp = !showHelp;
            });
        
        WALLSTICK = new ConsoleCommand<bool>("wallstick", "Enables the Player to Stick to Walls", "wallstick <bool>",
            (x) =>
            {
                player.EnableWallStick(x);
            });

        WALLJUMP = new ConsoleCommand<bool>("walljump", "Enables the Player to Jump off Walls", "walljump <bool>",
            (x) =>
            {
                player.EnableWallJump(x);
            });
        
        WALLCLIMB = new ConsoleCommand<bool>("wallclimb", "Enables the Player to climb Walls", "wallclimb <bool>",
            (x) =>
            {
                player.EnableWallClimb(x);
            });
        
        VARJUMP = new ConsoleCommand<bool>("variablejump", "Enables the Jump Input to result in variable Heights", "variablejump <bool>",
            (x) =>
            {
                player.EnableVariableJump(x);
            });
        
        DOUBLEJUMP = new ConsoleCommand<bool>("doublejump", "Enables the an additional Jump while airbourne", "doublejump <bool>",
            (x) =>
            {
                player.EnableDoubleJump(x);
            });
        
        commandList = new List<object>
        {
            HELP,
            WALLSTICK,
            WALLJUMP,
            WALLCLIMB,
            VARJUMP,
            DOUBLEJUMP
        };
    }
    private void OnGUI()
    {
        if (!showConsole)
            return;
        
        float y = 0;

        //ScrollBox
        if (showHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100),"");
            Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);
            scroll = GUI.BeginScrollView(new Rect(0, y + 5, Screen.width, 90), scroll, viewport);
            for (int i = 0; i < commandList.Count; i++)
            {
                ConsoleCommandBase command = commandList[i] as ConsoleCommandBase;
                string label = $"{command.commandFormat} - {command.commandDescription}";

                Rect labelRect = new Rect(5, 30 * i, viewport.width - 100, 20);
            
                GUI.Label(labelRect, label, myStyle);
            }
        
            GUI.EndScrollView();

            y += 100;
        }
        
        //InputField
        GUI.Box(new Rect(0, y, Screen.width, 50), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        GUI.SetNextControlName("inputField");
        input = GUI.TextField(new Rect(10, y + 10, Screen.width - 20, 25), input, myStyle);
 
        GUI.FocusControl("inputField");
    }
    private void HandleInput()
    {
        string[] properties = input.Split(' ');

        for (int i = 0; i < commandList.Count; i++)
        {
            ConsoleCommandBase commandBase = commandList[i] as ConsoleCommandBase;

            if (input.Contains(commandBase.commandID))
            {
                if (commandList[i] as ConsoleCommand != null)
                {
                    //Cast to this Type and Invoke
                    (commandList[i] as ConsoleCommand).Invoke();
                }
                else if (commandList[i] as ConsoleCommand<float> != null)
                {
                    (commandList[i] as ConsoleCommand<float>).Invoke(float.Parse(properties[1] + "f"));
                }
                else if (commandList[i] as ConsoleCommand<bool> != null)
                {
                    (commandList[i] as ConsoleCommand<bool>).Invoke(bool.Parse(properties[1]));
                }
            }
        }
    }
}
