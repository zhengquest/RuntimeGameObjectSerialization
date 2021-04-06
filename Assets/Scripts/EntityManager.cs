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
    private Dictionary<TypeReference, JobjectContainer> typeJobjectDict;
    
    private void Start()
    {
        path = Path.Combine(Application.persistentDataPath, "Test.txt");
        CreateTypeToJobjectDict();

        UiManager.CreateUiForObjects(inGameObjects, OnObjectSelectCallback);
        UiManager.CreateUiForBehaviours(typeJobjectDict);
        UiManager.SetupSaveCallback(SaveCustomizedEntity);
    }

    private void OnObjectSelectCallback(AssetReferenceGameObject goRef)
    {
        chosenInGameObject = goRef;
    }

    private void CreateTypeToJobjectDict()
    {
        typeJobjectDict = new Dictionary<TypeReference, JobjectContainer>(inGameBehaviours.Length);

        foreach (var inGameBehaviour in inGameBehaviours)
        {
            typeJobjectDict.Add(inGameBehaviour, CreateJobjectForBehaviour(inGameBehaviour));
        }
    }

    private JobjectContainer CreateJobjectForBehaviour(Type type)
    {
        var serializeFields = type.GetFields();

        var jProperties = new List<JProperty>(serializeFields.Length);
        foreach (var fieldInfo in serializeFields)
        {
            Debug.Log($"{fieldInfo}");
            jProperties.Add(new JProperty(fieldInfo.Name, 0));
        }

        return new JobjectContainer
        {
            jObject = new JObject(jProperties)
        };
    }

    public void SaveCustomizedEntity()
    {
        var savedEntityData = new SavedEntity
        {
            inGameObject = chosenInGameObject, savedBehaviours = new List<SavedBehaviour>()
        };
        
        foreach (var addedBehaviour in typeJobjectDict.Keys)
        {
            var jobjectContainer = typeJobjectDict[addedBehaviour];
            if (jobjectContainer.attachToEntity)
            {
                savedEntityData.savedBehaviours.Add(new SavedBehaviour
                {
                    behaviourType = addedBehaviour, behaviourData = jobjectContainer.jObject.ToString()
                });
            }
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
    public string behaviourData; 
}

[Serializable]
public class JobjectContainer
{
    public JObject jObject;
    public bool attachToEntity;
}
