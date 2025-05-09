using UnityEngine;

// WorldGenerator.cs
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private int seed;
    [SerializeField] private bool useRandomSeed = true;
    
    private System.Random random;
    private Dictionary<string, AreaData> generatedAreas = new Dictionary<string, AreaData>();
    
    public class AreaData
    {
        public string AreaID;
        public string AreaName;
        public GameEnums.EnvironmentType EnvironmentType;
        public int DifficultyLevel;
        public List<string> ConnectedAreas;
        public List<string> Npcs;
        public List<string> Bosses;
        public Vector2 Position;
        
        public AreaData(string id, string name, GameEnums.EnvironmentType envType, int difficulty)
        {
            AreaID = id;
            AreaName = name;
            EnvironmentType = envType;
            DifficultyLevel = difficulty;
            ConnectedAreas = new List<string>();
            Npcs = new List<string>();
            Bosses = new List<string>();
            Position = Vector2.zero;
        }
    }
    
    private void Awake()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(1, int.MaxValue);
        }
        
        random = new System.Random(seed);
        
        // Initialize starting area
        InitializeStartingArea();
    }
    
    public void Initialize(int worldSeed)
    {
        seed = worldSeed;
        random = new System.Random(seed);
        
        generatedAreas.Clear();
        InitializeStartingArea();
    }
    
    private void InitializeStartingArea()
    {
        AreaData startingArea = new AreaData(
            "starting_area",
            "Willow's Rest",
            GameEnums.EnvironmentType.City,
            1);
            
        startingArea.Position = Vector2.zero;
        
        // Add some connections
        startingArea.ConnectedAreas.Add("forest_edge");
        startingArea.ConnectedAreas.Add("abandoned_mine");
        
        // Add to generated areas
        generatedAreas.Add(startingArea.AreaID, startingArea);
        
        // Generate the connected areas
        GenerateArea("forest_edge", "Forest's Edge", GameEnums.EnvironmentType.Forest, 2, new Vector2(1, 0));
        GenerateArea("abandoned_mine", "Abandoned Mine", GameEnums.EnvironmentType.Cave, 2, new Vector2(0, 1));
    }
    
    private void GenerateArea(string id, string name, GameEnums.EnvironmentType envType, int difficulty, Vector2 position)
    {
        if (generatedAreas.ContainsKey(id))
            return;
            
        AreaData area = new AreaData(id, name, envType, difficulty);
        area.Position = position;
        
        // Add some random NPCs based on environment type
        AddRandomNpcs(area);
        
        // Add a boss if difficulty > 3
        if (difficulty > 3)
        {
            AddRandomBoss(area);
        }
        
        // Add connections to other areas
        AddRandomConnections(area);
        
        // Add to generated areas
        generatedAreas.Add(id, area);
    }
    
    private void AddRandomNpcs(AreaData area)
    {
        int npcCount = random.Next(1, 4); // 1-3 NPCs
        
        for (int i = 0; i < npcCount; i++)
        {
            string npcId = $"npc_{area.EnvironmentType.ToString().ToLower()}_{random.Next(1, 100)}";
            area.Npcs.Add(npcId);
        }
    }
    
    private void AddRandomBoss(AreaData area)
    {
        string bossId = $"boss_{area.EnvironmentType.ToString().ToLower()}_{random.Next(1, 10)}";
        area.Bosses.Add(bossId);
    }
    
    private void AddRandomConnections(AreaData area)
    {
        // Only add new connections if this is a higher level area
        if (area.DifficultyLevel <= 3)
            return;
            
        int connectionCount = random.Next(0, 3); // 0-2 additional connections
        
        for (int i = 0; i < connectionCount; i++)
        {
            // Create a new area to connect to
            string newAreaId = $"area_{area.EnvironmentType.ToString().ToLower()}_{random.Next(1, 1000)}";
            
            // Don't add if it already exists as a connection
            if (area.ConnectedAreas.Contains(newAreaId))
                continue;
                
            // Determine the environment type
            GameEnums.EnvironmentType newEnvType = (GameEnums.EnvironmentType)random.Next(
                0, System.Enum.GetValues(typeof(GameEnums.EnvironmentType)).Length);
                
            // Determine the difficulty (slightly higher)
            int newDifficulty = area.DifficultyLevel + random.Next(0, 2);
            
            // Determine position (adjacent to this area)
            Vector2 direction = new Vector2(
                random.Next(-1, 2), // -1, 0, or 1
                random.Next(-1, 2)  // -1, 0, or 1
            );
            
            // Don't allow (0,0) as a direction
            if (direction == Vector2.zero)
                direction = new Vector2(1, 0);
                
            Vector2 newPosition = area.Position + direction;
            
            // Add the connection
            area.ConnectedAreas.Add(newAreaId);
            
            // Generate the new area
            GenerateArea(
                newAreaId,
                GenerateAreaName(newEnvType),
                newEnvType,
                newDifficulty,
                newPosition);
        }
    }
    
    private string GenerateAreaName(GameEnums.EnvironmentType envType)
    {
        string[] prefixes = { "Northern", "Eastern", "Western", "Southern", "Upper", "Lower", "Hidden", "Ancient", "Mystic", "Haunted", "Forgotten" };
        string[] suffixes = { "Expanse", "Reaches", "Depths", "Heights", "Pass", "Crossing", "Ruins", "Dominion", "Vale", "Bastion" };
        
        string envName = envType.ToString();
        
        if (random.Next(0, 2) == 0)
        {
            // Format: [Prefix] [Environment]
            return $"{prefixes[random.Next(0, prefixes.Length)]} {envName}";
        }
        else
        {
            // Format: [Environment] [Suffix]
            return $"{envName} {suffixes[random.Next(0, suffixes.Length)]}";
        }
    }
    
    public AreaData GetArea(string areaId)
    {
        if (generatedAreas.TryGetValue(areaId, out AreaData area))
        {
            return area;
        }
        
        // Area doesn't exist, generate it dynamically
        // For now, create a random area
        GameEnums.EnvironmentType envType = (GameEnums.EnvironmentType)random.Next(
            0, System.Enum.GetValues(typeof(GameEnums.EnvironmentType)).Length);
            
        string areaName = GenerateAreaName(envType);
        
        // Determine difficulty based on format of areaId (assumed to contain a number)
        int difficulty = 1;
        string[] parts = areaId.Split('_');
        if (parts.Length > 1)
        {
            int.TryParse(parts[parts.Length - 1], out difficulty);
        }
        
        difficulty = Mathf.Clamp(difficulty, 1, 10);
        
        GenerateArea(areaId, areaName, envType, difficulty, Vector2.zero);
        
        return generatedAreas[areaId];
    }
    
    public Dictionary<string, AreaData> GetAllGeneratedAreas()
    {
        return new Dictionary<string, AreaData>(generatedAreas);
    }
    
    public List<string> GetConnectedAreas(string areaId)
    {
        AreaData area = GetArea(areaId);
        return area.ConnectedAreas;
    }
}