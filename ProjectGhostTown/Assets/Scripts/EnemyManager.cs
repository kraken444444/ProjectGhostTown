using UnityEngine;

// EnemyManager.cs - For spawning and managing enemies
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Singleton instance
    private static EnemyManager _instance;
    public static EnemyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("EnemyManager");
                _instance = obj.AddComponent<EnemyManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    // Enemy prefabs
    [SerializeField] private List<EnemyCharacter> enemyPrefabs = new List<EnemyCharacter>();
    
    // Active enemies
    private List<EnemyCharacter> activeEnemies = new List<EnemyCharacter>();
    
    // Enemy cache
    private Dictionary<string, EnemyCharacter> enemyCache = new Dictionary<string, EnemyCharacter>();
    
    // Spawn settings
    [SerializeField] private float minSpawnDistance = 5f;
    [SerializeField] private float maxSpawnDistance = 15f;
    [SerializeField] private int maxEnemiesPerArea = 10;
    [SerializeField] private float spawnCooldown = 5f;
    private float spawnTimer;
    
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
        
        // Cache enemies
        CacheEnemies();
    }
    
    private void Update()
    {
        // Check for enemy spawning
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && activeEnemies.Count < maxEnemiesPerArea)
        {
            TrySpawnEnemy();
            spawnTimer = spawnCooldown;
        }
        
        // Clean up destroyed enemies
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] == null || activeEnemies[i].IsDead())
            {
                activeEnemies.RemoveAt(i);
            }
        }
    }
    
    private void OnEnable()
    {
        // Subscribe to events
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnAreaChanged += OnAreaChanged;
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnAreaChanged -= OnAreaChanged;
        }
    }
    
    private void CacheEnemies()
    {
        // Clear cache
        enemyCache.Clear();
        
        // Cache prefabs
        foreach (var enemy in enemyPrefabs)
        {
            if (enemy != null)
            {
                enemyCache[enemy.name] = enemy;
            }
        }
        
        // Load from resources
        EnemyCharacter[] enemiesFromResources = Resources.LoadAll<EnemyCharacter>("Enemies");
        foreach (var enemy in enemiesFromResources)
        {
            if (enemy != null)
            {
                enemyCache[enemy.name] = enemy;
            }
        }
    }
    
    private void OnAreaChanged(string areaId)
    {
        // Clear active enemies
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null)
            {
                Destroy(activeEnemies[i].gameObject);
            }
        }
        
        activeEnemies.Clear();
        
        // Reset spawn timer
        spawnTimer = 0f;
    }
    
    private void TrySpawnEnemy()
    {
        if (GameState.Instance == null || string.IsNullOrEmpty(GameState.Instance.CurrentAreaID))
        {
            return;
        }
        
        // Get player position
        PlayerCharacter player = GameState.Instance.PlayerCharacter;
        if (player == null)
        {
            return;
        }
        
        // Determine enemy type based on area
        string enemyType = GetEnemyTypeForArea(GameState.Instance.CurrentAreaID);
        if (string.IsNullOrEmpty(enemyType))
        {
            return;
        }
        
        // Determine spawn position
        Vector2 spawnPosition = GetSpawnPosition(player.transform.position);
        
        // Spawn enemy
        EnemyCharacter enemy = SpawnEnemy(enemyType, spawnPosition);
        if (enemy != null)
        {
            activeEnemies.Add(enemy);
        }
    }
    
    private string GetEnemyTypeForArea(string areaId)
    {
        // TODO: Implement real logic based on area
        
        // For now, return a random enemy type
        string[] enemyTypes = new string[] { "Zombie", "Skeleton", "Spider", "Rat", "Slime" };
        return enemyTypes[Random.Range(0, enemyTypes.Length)];
    }
    
    private Vector2 GetSpawnPosition(Vector3 playerPosition)
    {
        // Get a random direction
        Vector2 direction = Random.insideUnitCircle.normalized;
        
        // Get a random distance
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
        
        // Calculate spawn position
        Vector2 spawnPosition = (Vector2)playerPosition + (direction * distance);
        
        // TODO: Check for valid spawn position (not in walls, etc.)
        
        return spawnPosition;
    }
    
    public EnemyCharacter SpawnEnemy(string enemyType, Vector2 position)
    {
        // Get enemy prefab
        EnemyCharacter prefab = GetEnemyPrefab(enemyType);
        if (prefab == null)
        {
            Debug.LogWarning($"Enemy prefab '{enemyType}' not found");
            return null;
        }
        
        // Instantiate enemy
        EnemyCharacter enemy = Instantiate(prefab, position, Quaternion.identity);
        
        // TODO: Initialize enemy based on area, player level, etc.
        
        return enemy;
    }
    
    public EnemyCharacter SpawnBoss(string bossType, Vector2 position)
    {
        // Get boss prefab
        EnemyCharacter prefab = GetEnemyPrefab(bossType);
        if (prefab == null)
        {
            Debug.LogWarning($"Boss prefab '{bossType}' not found");
            return null;
        }
        
        // Instantiate boss
        EnemyCharacter boss = Instantiate(prefab, position, Quaternion.identity);
        
        // TODO: Initialize boss stats, abilities, etc.
        
        return boss;
    }
    
    private EnemyCharacter GetEnemyPrefab(string enemyType)
    {
        // Get from cache if available
        if (enemyCache.TryGetValue(enemyType, out EnemyCharacter enemy))
        {
            return enemy;
        }
        
        // Try to load from resources
        EnemyCharacter enemyFromResources = Resources.Load<EnemyCharacter>($"Enemies/{enemyType}");
        if (enemyFromResources != null)
        {
            enemyCache[enemyType] = enemyFromResources;
            return enemyFromResources;
        }
        
        // Not found
        Debug.LogWarning($"Enemy '{enemyType}' not found");
        return null;
    }
    
    public List<EnemyCharacter> GetActiveEnemies()
    {
        return new List<EnemyCharacter>(activeEnemies);
    }
    
    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }
    
    public void ClearAllEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null)
            {
                Destroy(activeEnemies[i].gameObject);
            }
        }
        
        activeEnemies.Clear();
    }
}