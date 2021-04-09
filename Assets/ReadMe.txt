Built in Unity 2020.3.0f1, Android Platform, 1920 * 1080 Portrait

Key Points of my design:
1. use addressable AssetReference type to serialize in game object prefab references. This type has a very small size and thus much faster to serialize than the entire prefab file. (EntityManager.inGameObjects)
2. use TypeReference plugin to serialize in game behaviour types. I was gonna use prefab approach for behaviours as well but then I realized prefab is not a conceptual fit for behaviour as it must contains a transform component, which is wasteful. (EntityManager.inGameBehaviours)
3. use reflection to retrieve all public fields of the behaviour types. this maybe slower than writing a custom data class for each behaviour type but saves lots of tedious boilerplate code writing.
4. use Json.Net to create a JProperty for each retrieved field and a JObject for the entire behaviour object. Anytime a player customize a property in the behaviour in the Ui, it will be saved to the associated JProperty. (EntityManager.CreateJsonDataForBehaviour)
5. the JObject will then be serialized to disk on user saving entity. (EntityManager.SaveCustomizedEntity)

Improvements:
1. Even though this is not mentioned in the requirement, I added a quick and dirty way to check if a custom entity has been saved before by comparing serialized data. this is slow and can be easily made better with a proper comparison via IEquatable or IComparer. but I discovered this quite late and decided to use a make shift solution.
2. I was gonna use a much quicker serializer instead of the built in JsonUtility. but that one has issue serializing the AssetReference (so is Json.Net). this maybe the serializer's issue or a unity one. I would investigate further into the issue in a real project because serialization performance is vital to such a project. 
3. Ui can be better organized and written. Ideally use the new unity UI.