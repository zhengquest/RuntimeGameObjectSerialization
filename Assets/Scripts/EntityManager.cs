using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TypeReferences;
using static SerializeDataClasses;

public class EntityManager : MonoBehaviour
{
    public AssetReferenceGameObject[] inGameObjects;
    
    [Inherits(typeof(IBehaviour))] 
    public TypeReference[] inGameBehaviours;

    public CreatorUiManager creatorUiManager;
    public TestModeUiManager testUiManager;
    public GameObject floor;
    
    private string saveEntitiesPath;
    private AssetReferenceGameObject chosenInGameObject;
    private Dictionary<TypeReference, JsonData> userCustomDataByBehaviours;
    private SavedEntities savedEntities;

    private void Start()
    {
        saveEntitiesPath = Path.Combine(Application.persistentDataPath, "SavedEntities.txt");

        DeserializeSavedEntities();
        CreateUserDataByBehavioursDictionary();

        creatorUiManager.CreateUiForObjects(inGameObjects, OnObjectSelectCallback);
        creatorUiManager.CreateUiForBehaviours(userCustomDataByBehaviours);
        creatorUiManager.SetupSaveCallback(SaveCustomizedEntity);
        creatorUiManager.SetupTestModeCallback(SetupTestModeUi);
    }

    private void SetupTestModeUi()
    {
        var entities = GetSavedEntities();
        if (entities != null)
        {
            testUiManager.CreateUiForCustomEntities(entities, OnEnterCreateMode);
        }
    }

    private void OnEnterCreateMode()
    {
        creatorUiManager.ToggleVisible(true);
        for (var i = 0; i < floor.transform.childCount; i++)
        {
            Destroy(floor.transform.GetChild(i).gameObject);
        }
    }

    private void OnObjectSelectCallback(AssetReferenceGameObject goRef)
    {
        chosenInGameObject = goRef;
    }

    private void CreateUserDataByBehavioursDictionary()
    {
        userCustomDataByBehaviours = new Dictionary<TypeReference, JsonData>(inGameBehaviours.Length);

        foreach (var inGameBehaviour in inGameBehaviours)
        {
            userCustomDataByBehaviours.Add(inGameBehaviour, CreateJsonDataForBehaviour(inGameBehaviour));
        }
    }

    private IEnumerable<SavedEntity> GetSavedEntities()
    {
        DeserializeSavedEntities();
        return savedEntities.savedEntitiesList;
    }

    private JsonData CreateJsonDataForBehaviour(Type type)
    {
        var serializeFields = type.GetFields();
        var jProperties = new List<JProperty>(serializeFields.Length);
        
        jProperties.AddRange(serializeFields.Select(fieldInfo => new JProperty(fieldInfo.Name, 0)));

        return new JsonData { jObject = new JObject(jProperties) };
    }

    public void SaveCustomizedEntity()
    {
        if (chosenInGameObject == null)
        {
            Debug.Log("can't save due to no in game object chosen");
            return;
        }
        
        var savedEntityData = new SavedEntity
        {
            inGameObject = chosenInGameObject, savedBehaviours = new List<SavedBehaviour>()
        };
        
        foreach (var addedBehaviour in userCustomDataByBehaviours.Keys)
        {
            var jsonData = userCustomDataByBehaviours[addedBehaviour];
            if (jsonData.attachToEntity)
            {
                savedEntityData.savedBehaviours.Add(new SavedBehaviour
                {
                    behaviourType = addedBehaviour, behaviourData = jsonData.jObject.ToString()
                });
            }
        }
        
        savedEntities.savedEntitiesList.Add(savedEntityData);
        var serializedData = JsonUtility.ToJson(savedEntities, true);
        File.WriteAllText(saveEntitiesPath, serializedData);

        //Debug.Log(serializedData);
    }

    public void DeserializeSavedEntities()
    {
        if (!File.Exists(saveEntitiesPath))
        {
            savedEntities = new SavedEntities();
            return;
        }
        
        var data = File.ReadAllText(saveEntitiesPath);
        savedEntities = JsonUtility.FromJson(data, typeof(SavedEntities)) as SavedEntities;
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
