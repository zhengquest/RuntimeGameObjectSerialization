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
    public Button saveButton;
    private Dictionary<TabButton, GameObject> tabPropertyDict;
    
    public void CreateUiForBehaviour(Dictionary<TypeReference, JObject> inGameBehaviours)
    {
        tabPropertyDict = new Dictionary<TabButton, GameObject>(inGameBehaviours.Count);
        
        foreach (var behaviour in inGameBehaviours)
        {
            var newTabBtn = Instantiate(behaviourButtonUi, behaviourTab);
            var newPropertyUi = Instantiate(behaviourPropertyUi, behaviourSection);
            tabPropertyDict.Add(newTabBtn, newPropertyUi);
            
            foreach (var fieldInfo in behaviour.Key.Type.GetFields())
            {
                CreateUiForBehaviourField(fieldInfo, newPropertyUi.transform, behaviour.Value);
            }
        }

        OnTabButtonClick(tabPropertyDict.Keys.First());
    }

    public void OnTabButtonClick(TabButton tabButtonClicked)
    {
        foreach (var tab in tabPropertyDict.Keys)
        {
            tabPropertyDict[tab].SetActive(tab == tabButtonClicked);
        }
    }

    public void CreateUiForBehaviourField(FieldInfo fieldInfo, Transform parent, JObject associatedJobject)
    {
        if (fieldInfo.FieldType == typeof(int))
        {
            Instantiate(sliderUi, parent).SetupSliderForIntType(fieldInfo.Name, 100, associatedJobject.Property(fieldInfo.Name));
        }
        else if (fieldInfo.FieldType == typeof(float))
        {
            Instantiate(sliderUi, parent).SetupSliderForFloatType(fieldInfo.Name, 100f, associatedJobject.Property(fieldInfo.Name));
        }
        else if (fieldInfo.FieldType == typeof(string))
        { 
            Instantiate(TextUi, parent).SetupTextUi(fieldInfo.Name, "", associatedJobject.Property(fieldInfo.Name));
        }
    }

    public void SetupSaveCallback(Action saveCustomizedEntity)
    {
        saveButton.onClick.AddListener(() => { saveCustomizedEntity(); });
    }
}
