using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SliderUi : MonoBehaviour
{
    [SerializeField] private Text textLabel;
    [SerializeField] private Slider slider;

    public void SetupSliderForIntType(string label, int max, JProperty associatedProperty)
    {
        textLabel.text = label;
        slider.wholeNumbers = true;
        slider.maxValue = max;
        slider.onValueChanged.AddListener(updatedVal => associatedProperty.Value = updatedVal);
    }
    
    public void SetupSliderForFloatType(string label, float max, JProperty associatedProperty)
    {
        textLabel.text = label;
        slider.wholeNumbers = false;
        slider.maxValue = max;
        slider.onValueChanged.AddListener(updatedVal => associatedProperty.Value = updatedVal);
    }
}
