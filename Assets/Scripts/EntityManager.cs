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
    private AssetReferenceGameObject chosenInGameObject;
    private TypeReference chosenInGameBehaviour;
    private Dictionary<TypeReference, JObject> typeJobjectDict;
    
    private void Start()
    {
        path = Path.Combine(Application.persistentDataPath, "Test.txt");
        chosenInGameObject = inGameObjects[0];
        chosenInGameBehaviour = inGameBehaviours[0];
        CreateTypeToJobjectDict();

        UiManager.CreateUiForObjects(inGameObjects, OnObjectSelectCallback);
        UiManager.CreateUiForBehaviours(typeJobjectDict);
        UiManager.SetupSaveCallback(SaveCustomizedEntity);
    }

    private void OnObjectSelectCallback(AssetReferenceGameObject goRef)
    {
        chosenInGameObject = goRef;
        Debug.Log($"selectied gameobject {chosenInGameObject}");
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

    public void SaveCustomizedEntity()
    {
        var savedEntityData = new SavedEntity
        {
            inGameObject = chosenInGameObject, savedBehaviours = new List<SavedBehaviour>()
        };
        
        foreach (var addedBehaviour in typeJobjectDict.Keys)
        {
            savedEntityData.savedBehaviours.Add(new SavedBehaviour
            {
                behaviourType = addedBehaviour, behaviourData = typeJobjectDict[addedBehaviour].ToString()
            });
        }

        var serializedData = JsonUtility.ToJson(savedEntityData, true);
        Debug.Log(serializedData);
        File.WriteAllText(path, serializedData);

        DeserializeSavedEntity();
    }

    public void DeserializeSavedEntity()
    {
        var data = File.ReadAllText(path);
        var entity = JsonUtility.FromJson(data, typeof(SavedEntity)) as SavedEntity;
        Debug.Log(entity);

        SpawnEntityAsync(entity);
    }

    private async void SpawnEntityAsync(SavedEntity savedEntity) 
    {
        var go = await savedEntity.inGameObject.InstantiateAsync().Task;

        foreach (var savedBehaviour in savedEntity.savedBehaviours)
        {
            var behaviour = go.AddComponent(savedBehaviour.behaviourType);
            JsonUtility.FromJsonOverwrite(savedBehaviour.behaviourData, behaviour);            
        }
    }
}

[Serializable]
public class SavedEntity
{
    public AssetReferenceGameObject inGameObject;
    public List<SavedBehaviour> savedBehaviours;
}

[Serializable]
public class SavedBehaviour
{
    public TypeReference behaviourType;
    public string behaviourData; //json or pure string or bytes
}
