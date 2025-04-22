using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    //singleton pattern for easy access
    public static GameState Instance { get; private set; }

    //player data
    public Character PlayerCharacter { get; private set; }
    public CardDeck PlayerDeck { get; private set; }
    public HashSet<string> DiscoveredAreas { get; private set; } = new HashSet<string>();
    public HashSet<string> DefeatedBosses { get; private set; } = new HashSet<string>();
    public HashSet<string> ActivatedMonoliths { get; private set; } = new HashSet<string>();

    //world state
    public WorldGenerator WorldGenerator { get; private set; }
    public string CurrentAreaID { get; private set; }
    public int WorldSeed { get; private set; }
    
    public float PlayTime { get; private set; }
    
    private void Awake()
    {
        //singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        DiscoveredAreas = new HashSet<string>();
        DefeatedBosses = new HashSet<string>();
        ActivatedMonoliths = new HashSet<string>();
    }

    public void InitializeNewGame(Character character, int worldSeed)
    {
        PlayerCharacter = character;
        PlayerDeck = new CardDeck();
        WorldSeed = worldSeed;
        PlayTime = 0f;
        
        DiscoveredAreas.Clear();
        DefeatedBosses.Clear();
        ActivatedMonoliths.Clear();
        
        DiscoveredAreas.Add("starting_area");
        CurrentAreaID = "starting_area";
        
        //initialize world
        WorldGenerator = new WorldGenerator();
    }
    
    public void LoadGame(SaveData saveData)
    {
        //populate from save data
        PlayerCharacter = saveData.PlayerCharacter;
        PlayerDeck = saveData.PlayerDeck;
        DiscoveredAreas = new HashSet<string>(saveData.DiscoveredAreas);
        DefeatedBosses = new HashSet<string>(saveData.DefeatedBosses);
        ActivatedMonoliths = new HashSet<string>(saveData.ActivatedMonoliths);
        WorldSeed = saveData.WorldSeed;
        CurrentAreaID = saveData.CurrentAreaID;
        PlayTime = saveData.PlayTime;
        
        WorldGenerator = new WorldGenerator();
    }
    
    public SaveData CreateSaveData()
    {
        SaveData saveData = new SaveData
        {
            PlayerCharacter = PlayerCharacter,
            PlayerDeck = PlayerDeck,
            DiscoveredAreas = new List<string>(DiscoveredAreas),
            DefeatedBosses = new List<string>(DefeatedBosses),
            ActivatedMonoliths = new List<string>(ActivatedMonoliths),
            WorldSeed = WorldSeed,
            CurrentAreaID = CurrentAreaID,
            PlayTime = PlayTime
        };
        
        return saveData;
    }
    
    private void Update()
    {
        PlayTime += Time.deltaTime;
    }
    
    public void DiscoverArea(string areaID)
    {
        DiscoveredAreas.Add(areaID);
    }
    
    public void DefeatBoss(string bossID)
    {
        DefeatedBosses.Add(bossID);
    }
    
    public void ActivateMonolith(string monolithID)
    {
        ActivatedMonoliths.Add(monolithID);
    }
    
    public void ChangeArea(string newAreaID)
    {
        CurrentAreaID = newAreaID;
        DiscoverArea(newAreaID);
    }
}


