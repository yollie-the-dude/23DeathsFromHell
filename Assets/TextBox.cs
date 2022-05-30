using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    public TMP_InputField field;

    private void Update()
    {
       field.ActivateInputField();

    }
    void OnGUI()
    {
        char chr = Event.current.character;
        if ((chr < 'a' || chr > 'z') && (chr < 'A' || chr > 'Z') && (chr < '0' || chr > '9'))
        {
            Event.current.character = '\0';
        }
        field.text = GUILayout.TextField(field.text, GUILayout.Width(0));
    }
}
