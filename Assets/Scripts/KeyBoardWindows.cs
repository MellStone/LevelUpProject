using UnityEngine;
using TMPro;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class KeyBoardWindows : MonoBehaviour
{
    public TMP_InputField inputField;
    public bool ShowWindowsKeyboard = false;

    private void Start()
    {
        inputField.onSelect.AddListener(OnInputSelected);
    }

    private void OnInputSelected(string text)
    {
        if (ShowWindowsKeyboard)
        {
            ShowOnScreenKeyboard();
        }
    }

    void ShowOnScreenKeyboard()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // Check if OSK is already running
        Process[] oskProcesses = Process.GetProcessesByName("osk");
        if (oskProcesses.Length == 0)
        {
            try
            {
                Process.Start("osk.exe");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError("Failed to open OSK: " + ex.Message);
            }
        }
#endif
    }
}

