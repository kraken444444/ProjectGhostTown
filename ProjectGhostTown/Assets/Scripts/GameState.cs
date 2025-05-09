// GameState.cs
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    //singleton pattern for easy access
    public static GameState Instance { get; private set; }

    //player data
    public PlayerCharacter PlayerCharacter { get; private set; }
    
    public CharacterClass PlayerCharacterClass { get; private set; }
    
    public int PlayerLevel { get; private set; }
    
    public int PlayerExperience { get; private set; }
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

    public void InitializeNewGame(PlayerCharacter character, int worldSeed)
    {
        if (character == null)
        {
            Debug.LogError("Cannot initialize game with null character");
            return;
        }
        
        PlayerCharacter = character;
        
        CharacterClass characterClass = character.GetCharacterClass();
        if (characterClass != null)
        {
            PlayerCharacterClass = characterClass;
            
            // Initialize deck with starting cards
            if (PlayerDeck == null)
            {
                PlayerDeck = gameObject.AddComponent<CardDeck>();
            }
            
            if (characterClass.StartingCards != null)
            {
                foreach (var cardTemplate in characterClass.StartingCards)
                {
                    if (cardTemplate != null)
                    {
                        Card newCard = cardTemplate.CreateCardInstance();
                        PlayerDeck.AddCardToDeck(newCard);
                    }
                }
            }
        }
        
        WorldSeed = worldSeed;
        PlayTime = 0f;
        
        DiscoveredAreas.Clear();
        DefeatedBosses.Clear();
        ActivatedMonoliths.Clear();
        
        DiscoveredAreas.Add("starting_area");
        CurrentAreaID = "starting_area";
        
        // Initialize world
        if (WorldGenerator == null)
        {
            WorldGenerator = gameObject.AddComponent<WorldGenerator>();
        }
    }
    
    public void LoadGame(SaveData saveData)
    {
        if (saveData == null)
        {
            Debug.LogError("Cannot load game from null save data");
            return;
        }
        
        //populate from save data
        PlayerCharacter = saveData.PlayerCharacter;
        
        if (saveData.PlayerDeck != null)
        {
            PlayerDeck = saveData.PlayerDeck;
        }
        else
        {
            Debug.LogWarning("Save data contains null deck, creating new deck");
            PlayerDeck = gameObject.AddComponent<CardDeck>();
        }
        
        if (PlayerCharacter != null)
        {
            PlayerCharacterClass = PlayerCharacter.GetCharacterClass();
        }
        else
        {
            Debug.LogError("Save data contains null PlayerCharacter");
        }
        
        DiscoveredAreas = new HashSet<string>(saveData.DiscoveredAreas ?? new List<string>());
        DefeatedBosses = new HashSet<string>(saveData.DefeatedBosses ?? new List<string>());
        ActivatedMonoliths = new HashSet<string>(saveData.ActivatedMonoliths ?? new List<string>());
        WorldSeed = saveData.WorldSeed;
        CurrentAreaID = saveData.CurrentAreaID ?? "starting_area";
        PlayTime = saveData.PlayTime;
        
        if (WorldGenerator == null)
        {
            WorldGenerator = gameObject.AddComponent<WorldGenerator>();
        }
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
        if (!string.IsNullOrEmpty(areaID))
        {
            DiscoveredAreas.Add(areaID);
        }
    }
    
    public void DefeatBoss(string bossID)
    {
        if (!string.IsNullOrEmpty(bossID))
        {
            DefeatedBosses.Add(bossID);
        }
    }
    
    public void ActivateMonolith(string monolithID)
    {
        if (!string.IsNullOrEmpty(monolithID))
        {
            ActivatedMonoliths.Add(monolithID);
        }
    }
    
    public void ChangeArea(string newAreaID)
    {
        if (!string.IsNullOrEmpty(newAreaID))
        {
            CurrentAreaID = newAreaID;
            DiscoverArea(newAreaID);
        }
    }
}
