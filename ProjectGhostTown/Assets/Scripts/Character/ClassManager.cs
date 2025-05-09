// ClassManager.cs - New class for managing character classes
using System.Collections.Generic;
using UnityEngine;

public class ClassManager : MonoBehaviour
{
    // Singleton instance
    private static ClassManager _instance;
    public static ClassManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("ClassManager");
                _instance = obj.AddComponent<ClassManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    // Class data cache
    private Dictionary<GameEnums.ClassType, CharacterClassData> classDataCache = new Dictionary<GameEnums.ClassType, CharacterClassData>();
    private Dictionary<GameEnums.ClassType, CharacterClass> classCache = new Dictionary<GameEnums.ClassType, CharacterClass>();
    
    private void Awake()
    {
        // Handle singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Pre-cache the class data
        PreloadClassData();
    }
    
    private void PreloadClassData()
    {
        // Preload all class data
        classDataCache[GameEnums.ClassType.Radiomancer] = ClassFactory.CreateRadiomancerData();
        classDataCache[GameEnums.ClassType.Brawler] = ClassFactory.CreateBrawlerData();
        classDataCache[GameEnums.ClassType.Gunslinger] = ClassFactory.CreateGunslingerData();
        classDataCache[GameEnums.ClassType.Gambler] = ClassFactory.CreateGamblerData();
        
        // Create class instances
        foreach (var pair in classDataCache)
        {
            classCache[pair.Key] = CharacterClass.FromClassData(pair.Value);
        }
    }
    
    public CharacterClassData GetClassData(GameEnums.ClassType classType)
    {
        // Return from cache if available
        if (classDataCache.TryGetValue(classType, out CharacterClassData data))
        {
            return data;
        }
        
        // Generate data if not in cache
        CharacterClassData newData = CharacterClassData.CreateFromType(classType);
        classDataCache[classType] = newData;
        return newData;
    }
    
    public CharacterClass GetClass(GameEnums.ClassType classType)
    {
        // Return from cache if available
        if (classCache.TryGetValue(classType, out CharacterClass characterClass))
        {
            return characterClass;
        }
        
        // Generate class if not in cache
        CharacterClassData data = GetClassData(classType);
        CharacterClass newClass = CharacterClass.FromClassData(data);
        classCache[classType] = newClass;
        return newClass;
    }
    
    public void InitializePlayerClassData(PlayerCharacter player, GameEnums.ClassType classType)
    {
        if (player == null)
        {
            Debug.LogError("Cannot initialize class for null player");
            return;
        }
        
        CharacterClass characterClass = GetClass(classType);
        
        // Set player attributes from class data
        Attributes attributes = player.GetAttributes();
        if (attributes != null && characterClass != null && characterClass.BaseAttributeModifiers != null)
        {
            foreach (var pair in characterClass.BaseAttributeModifiers)
            {
                attributes[pair.Key] = pair.Value;
            }
        }
        
        // TODO: Initialize additional class-specific data on the player
    }
}
