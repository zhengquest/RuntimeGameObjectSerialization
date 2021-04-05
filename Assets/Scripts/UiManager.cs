using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using TypeReferences;
using UnityEngine;
using UnityEngine.UI;

public class UiManager: MonoBehaviour
{
    public TextUi TextUi;
    public SliderUi sliderUi;
    public GameObject behaviourPropertyUi;
    public TabButton behaviourButtonUi;
    public RectTransform behaviourTab;
    public RectTransform behaviourSection;

    private Dictionary<TabButton, GameObject> tabPropertyDict;
    
    public void CreateUiForBehaviour(TypeReference[] inGameBehaviours)
    {
        tabPropertyDict = new Dictionary<TabButton, GameObject>(inGameBehaviours.Length);
        
        foreach (var behaviour in inGameBehaviours)
        {
            var newTabBtn = Instantiate(behaviourButtonUi, behaviourTab);
            var newPropertyUi = Instantiate(behaviourPropertyUi, behaviourSection);
            tabPropertyDict.Add(newTabBtn, newPropertyUi);
            
            foreach (var fieldInfo in behaviour.Type.GetFields())
            {
                CreateUiForBehaviourField(fieldInfo, newPropertyUi.transform);
            }
        }

        OnTabButtonClick(tabPropertyDict.Keys.First());
        // foreach (var propertyUi in tabPropertyDict.Values)
        // {
        //     propertyUi.SetActive(false);
        // }
        //
        // tabPropertyDict.Values.First().SetActive(true);
    }

    public void OnTabButtonClick(TabButton tabButtonClicked)
    {
        foreach (var tab in tabPropertyDict.Keys)
        {
            tabPropertyDict[tab].SetActive(tab == tabButtonClicked);
        }
    }


    public void CreateUiForBehaviourField(FieldInfo fieldInfo, Transform parent, Action<JObject> valueChangeCallback = null)
    {
        if (fieldInfo.FieldType == typeof(int))
        {
            Instantiate(sliderUi, parent).SetupSliderForIntType(fieldInfo.Name, 100);
        }
        else if (fieldInfo.FieldType == typeof(float))
        {
            Instantiate(sliderUi, parent).SetupSliderForFloatType(fieldInfo.Name, 100f);
        }
        else if (fieldInfo.FieldType == typeof(string))
        { 
            Instantiate(TextUi, parent).SetupTextUi(fieldInfo.Name, "");
        }
    }
}
