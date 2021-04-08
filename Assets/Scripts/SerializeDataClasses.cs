using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TypeReferences;
using UnityEngine.AddressableAssets;

public abstract class SerializeDataClasses
{
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
    public class JsonData
    {
        public JObject jObject;
        public bool attachToEntity;
    }
}