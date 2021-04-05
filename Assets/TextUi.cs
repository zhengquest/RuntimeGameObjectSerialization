using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUi : MonoBehaviour
{
    [SerializeField] private Text textLabel;
    [SerializeField] private InputField textValue;

    public void SetupTextUi(string label, string initValue)
    {
        textLabel.text = label;
        textValue.text = initValue;
    }
}
