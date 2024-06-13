using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleCommandBase
{
    public string commandID { private set; get; }
    public string commandDescription{ private set; get; }
    public string commandFormat{ private set; get; }

    public ConsoleCommandBase(string id, string description, string format)
    {
        commandID = id;
        commandDescription = description;
        commandFormat = format;
    }
}

public class ConsoleCommand : ConsoleCommandBase
{
    private Action command;

    public ConsoleCommand(string id, string description, string format, Action command) : base(id, description, format)
    {
        this.command = command;
    }
    public void Invoke()
    {
        command.Invoke();
    }
}

public class ConsoleCommand<T1> : ConsoleCommandBase
{
    private Action<T1> command;

    public ConsoleCommand(string id, string description, string format, Action<T1> command) : base(id, description, format)
    {
        this.command = command;
    }
    public void Invoke(T1 value)
    {
        command.Invoke(value);
    }
}