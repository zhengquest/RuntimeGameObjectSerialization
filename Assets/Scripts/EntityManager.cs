using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TypeReferences;

public class EntityManager : MonoBehaviour
{
    public AssetReferenceGameObject[] inGameObjects;
    
    [Inherits(typeof(IBehaviour))] 
    public TypeReference[] inGameBehaviours;

    public UiManager UiManager;
    
    private string path;
    private Dictionary<TypeReference, JObject> typeJobjectDict;
    
    private void Start()
    {
        path = Path.Combine(Application.persistentDataPath, "Test.txt");

        CreateTypeToJobjectDict();
        
        UiManager.CreateUiForBehaviour(typeJobjectDict);
    }

    private void CreateTypeToJobjectDict()
    {
        typeJobjectDict = new Dictionary<TypeReference, JObject>(inGameBehaviours.Length);

        foreach (var inGameBehaviour in inGameBehaviours)
        {
            typeJobjectDict.Add(inGameBehaviour, CreateJobjectForBehaviour(inGameBehaviour));
        }
    }

    private JObject CreateJobjectForBehaviour(Type type)
    {
        var serializeFields = type.GetFields();

        var jProperties = new List<JProperty>(serializeFields.Length);
        foreach (var fieldInfo in serializeFields)
        {
            Debug.Log($"{fieldInfo}");
            jProperties.Add(new JProperty(fieldInfo.Name, 0));
        }

        return new JObject(jProperties);
    }

}

