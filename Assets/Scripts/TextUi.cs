using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TextUi : MonoBehaviour
{
    [SerializeField] private Text textLabel;
    [SerializeField] private InputField textValue;

    public void SetupTextUi(string label, string initValue, JProperty associatedProperty)
    {
        textLabel.text = label;
        textValue.text = initValue;
        textValue.onValueChanged.AddListener(updatedVal => associatedProperty.Value = updatedVal);
    }

}
