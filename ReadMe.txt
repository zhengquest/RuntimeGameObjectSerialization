Built in Unity 2020.3.5f1, Android Platform, 1920 * 1080 Portrait

summary:
This is a runtime serialization system designed for saving player customized entities during gameplay (for example, a survival game where player build a custom house with given parts).
Player can choose one of the two gameobjects and assign one or more behaviours to it (explose or add points). The behaviours have parameters for customization. Press save, The created entity is serialized and saved for later use. Player can then go into test mode to test the entities that are just created. clicking on them to see the behaviours executed with the saved parameters.

notes:
1. use addressable AssetReference type to serialize in game object prefab references. This type has a very small size and thus much faster to serialize than the entire prefab file. (EntityManager.inGameObjects)
2. use TypeReference plugin to serialize in game behaviour types. I was gonna use prefab approach for behaviours as well but then I realized prefab is not a conceptual fit for behaviour as it must contains a transform component, which is wasteful. (EntityManager.inGameBehaviours)
3. use reflection to retrieve all public fields of the behaviour types. this maybe slower than writing a custom data class for each behaviour type but saves lots of tedious boilerplate code writing.
4. use Json.Net to create a JProperty for each retrieved field and a JObject for the entire behaviour object. Anytime a player customize a property in the behaviour in the Ui, it will be saved to the associated JProperty. (EntityManager.CreateJsonDataForBehaviour)
5. the JObject will then be serialized to disk on user saving entity. (EntityManager.SaveCustomizedEntity)
