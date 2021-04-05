using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderUi : MonoBehaviour
{
    [SerializeField] private Text textLabel;
    [SerializeField] private Slider slider;

    public void SetupSliderForIntType(string label, int max)
    {
        textLabel.text = label;
        slider.wholeNumbers = true;
        slider.maxValue = max;
    }
    
    public void SetupSliderForFloatType(string label, float max)
    {
        textLabel.text = label;
        slider.wholeNumbers = false;
        slider.maxValue = max;
    }
}
