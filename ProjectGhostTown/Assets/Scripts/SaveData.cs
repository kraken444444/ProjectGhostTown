using System.Collections.Generic;
using UnityEngine;

// SaveData.cs - Serializable data for saving/loading
[System.Serializable]
public class SaveData
{
    public Character PlayerCharacter;
    public CardDeck PlayerDeck;
    public List<string> DiscoveredAreas;
    public List<string> DefeatedBosses;
    public List<string> ActivatedMonoliths;
    public int WorldSeed;
    public string CurrentAreaID;
    public int GameDifficulty;
    public float PlayTime;
}