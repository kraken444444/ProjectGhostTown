using UnityEngine;

// SpellFactory.cs - Factory for creating spell instances
using System.Collections.Generic;
using UnityEngine;

public class SpellFactory : MonoBehaviour
{
    // Singleton instance
    private static SpellFactory _instance;
    public static SpellFactory Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("SpellFactory");
                _instance = obj.AddComponent<SpellFactory>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    // Spell prefabs
    [SerializeField] private List<Spell> spellPrefabs = new List<Spell>();
    
    // Spell cache
    private Dictionary<string, Spell> spellCache = new Dictionary<string, Spell>();
    
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
        
        // Cache spells
        CacheSpells();
    }
    
    private void CacheSpells()
    {
        // Clear cache
        spellCache.Clear();
        
        // Cache prefabs
        foreach (var spell in spellPrefabs)
        {
            if (spell != null)
            {
                spellCache[spell.spellName] = spell;
            }
        }
        
        // Load from resources
        Spell[] spellsFromResources = Resources.LoadAll<Spell>("Spells");
        foreach (var spell in spellsFromResources)
        {
            if (spell != null)
            {
                spellCache[spell.spellName] = spell;
            }
        }
    }
    
    public Spell GetSpell(string spellName)
    {
        // Get from cache if available
        if (spellCache.TryGetValue(spellName, out Spell spell))
        {
            return spell;
        }
        
        // Try to load from resources
        Spell spellFromResources = Resources.Load<Spell>($"Spells/{spellName}");
        if (spellFromResources != null)
        {
            spellCache[spellName] = spellFromResources;
            return spellFromResources;
        }
        
        // Not found
        Debug.LogWarning($"Spell '{spellName}' not found");
        return null;
    }
    
    public List<Spell> GetSpellsByClass(GameEnums.ClassType classType, int maxLevel = int.MaxValue)
    {
        List<Spell> result = new List<Spell>();
        
        foreach (var spell in spellCache.Values)
        {
            if (spell.classRequirement == classType && spell.levelRequirement <= maxLevel)
            {
                result.Add(spell);
            }
        }
        
        return result;
    }
    
    public List<Spell> GetSpellsByType(GameEnums.SpellType spellType, int maxLevel = int.MaxValue)
    {
        List<Spell> result = new List<Spell>();
        
        foreach (var spell in spellCache.Values)
        {
            if (spell.spellType == spellType && spell.levelRequirement <= maxLevel)
            {
                result.Add(spell);
            }
        }
        
        return result;
    }
    
    public List<Spell> GetSpellsByTag(string tag, int maxLevel = int.MaxValue)
    {
        List<Spell> result = new List<Spell>();
        
        foreach (var spell in spellCache.Values)
        {
            if (spell.tags != null && spell.tags.Contains(tag) && spell.levelRequirement <= maxLevel)
            {
                result.Add(spell);
            }
        }
        
        return result;
    }
    
    public Spell CreateSpell(string spellName)
    {
        Spell template = GetSpell(spellName);
        if (template == null)
        {
            return null;
        }
        
        // Create a copy of the template
        Spell newSpell = Instantiate(template);
        return newSpell;
    }
}