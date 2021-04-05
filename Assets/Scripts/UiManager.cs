using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UiManager: MonoBehaviour
{
    public TextUi TextUi;
    public SliderUi sliderUi;
    public Canvas behaviourUi;
    
    public void CreateUiForType(FieldInfo fieldInfo, Action<JObject> valueChangeCallback = null)
    {
        if (fieldInfo.FieldType == typeof(int))
        {
            Instantiate(sliderUi, behaviourUi.transform).SetupSliderForIntType(fieldInfo.Name, 100);
        }
        else if (fieldInfo.FieldType == typeof(float))
        {
            Instantiate(sliderUi, behaviourUi.transform).SetupSliderForFloatType(fieldInfo.Name, 100f);
        }
        else if (fieldInfo.FieldType == typeof(string))
        { 
            Instantiate(TextUi, behaviourUi.transform).SetupTextUi(fieldInfo.Name, "");
        }
    }
}
