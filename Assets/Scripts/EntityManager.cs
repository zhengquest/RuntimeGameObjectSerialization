using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TypeReferences;

public class EntityManager : MonoBehaviour
{
    public AssetReferenceGameObject[] inGameObjects;
    
    [Inherits(typeof(IBehaviour))] 
    public TypeReference[] inGameBehaviours;

    public CreatorUiManager creatorUiManager;
    public TestModeUiManager testUiManager;
    public GameObject floor;
    
    private string path;
    private AssetReferenceGameObject chosenInGameObject;
    private Dictionary<TypeReference, JobjectContainer> typeJobjectDict;
    private SavedEntities entities;

    private void Start()
    {
        path = Path.Combine(Application.persistentDataPath, "SavedEntities.txt");

        DeserializeSavedEntities();
        CreateTypeToJobjectDict();

        creatorUiManager.CreateUiForObjects(inGameObjects, OnObjectSelectCallback);
        creatorUiManager.CreateUiForBehaviours(typeJobjectDict);
        creatorUiManager.SetupSaveCallback(SaveCustomizedEntity);
        creatorUiManager.SetupTestModeCallback(TestModeBtnCallback);
    }

    private void TestModeBtnCallback()
    {
        SetupTestModeUi();
        testUiManager.ToggleVisible(true);
    }

    private void SetupTestModeUi()
    {
        var savedEntites = GetSavedEntities();
        if (savedEntites != null)
        {
            testUiManager.CreateUiForCustomEntities(savedEntites, OnEnterCreateMode);
        }
    }

    private void OnEnterCreateMode()
    {
        creatorUiManager.ToggleVisible(true);
        for (int i = 0; i < floor.transform.childCount; i++)
        {
            Destroy(floor.transform.GetChild(i).gameObject);
        }
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

    private IEnumerable<SavedEntity> GetSavedEntities()
    {
        DeserializeSavedEntities();
        return entities.savedEntitiesList;
    }

    private JobjectContainer CreateJobjectForBehaviour(Type type)
    {
        var serializeFields = type.GetFields();
        var jProperties = new List<JProperty>(serializeFields.Length);
        
        jProperties.AddRange(serializeFields.Select(fieldInfo => new JProperty(fieldInfo.Name, 0)));

        return new JobjectContainer { jObject = new JObject(jProperties) };
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
        
        entities.savedEntitiesList.Add(savedEntityData);
        var serializedData = JsonUtility.ToJson(entities, true);
        File.WriteAllText(path, serializedData);

        Debug.Log(serializedData);
    }

    public void DeserializeSavedEntities()
    {
        if (!File.Exists(path))
        {
            entities = new SavedEntities();
            return;
        }
        
        var data = File.ReadAllText(path);
        entities = JsonUtility.FromJson(data, typeof(SavedEntities)) as SavedEntities;
    }

    public async void SpawnEntityAsync(SavedEntity savedEntity, Vector3 spawnPoint) 
    {
        var go = await savedEntity.inGameObject.InstantiateAsync(floor.transform).Task;
        go.transform.position = spawnPoint;
        
        foreach (var savedBehaviour in savedEntity.savedBehaviours)
        {
            var behaviour = go.AddComponent(savedBehaviour.behaviourType);
            JsonUtility.FromJsonOverwrite(savedBehaviour.behaviourData, behaviour);            
        }
    }
}

[Serializable]
public class SavedEntities
{
    public List<SavedEntity> savedEntitiesList = new List<SavedEntity>();
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
