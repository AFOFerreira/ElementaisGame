using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogCalback : MonoBehaviour
{
    public string output = "";
    public string stack = "";
    public InputField input;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        DontDestroyOnLoad(this.gameObject);
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;

        input.text += type + ":- "+logString+"\n"+stackTrace+"\n\n";
    }
}
