using UnityEngine;

// PlayerProgressionManager.cs - Manages player progression, skills, etc.
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgressionManager : MonoBehaviour
{
    // Singleton instance
    private static PlayerProgressionManager _instance;
    public static PlayerProgressionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("PlayerProgressionManager");
                _instance = obj.AddComponent<PlayerProgressionManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    // Player skill points
    private int availableSkillPoints = 0;
    
    // Unlocked skills
    private HashSet<string> unlockedSkills = new HashSet<string>();
    
    // Skill tree
    private SkillTree skillTree;
    
    // Level up rewards
    [SerializeField] private int skillPointsPerLevel = 1;
    [SerializeField] private int attributePointsPerLevel = 2;
    
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
        
        // Find skill tree
        skillTree = FindObjectOfType<SkillTree>();
    }
    
    private void OnEnable()
    {
        // Subscribe to events
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnPlayerLevelUp += OnPlayerLevelUp;
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnPlayerLevelUp -= OnPlayerLevelUp;
        }
    }
    
    private void OnPlayerLevelUp(int newLevel)
    {
        // Award skill points
        availableSkillPoints += skillPointsPerLevel;
        
        // Award attribute points
        if (GameState.Instance != null && GameState.Instance.PlayerCharacter != null)
        {
            // TODO: Add attribute points to player
        }
        
        // Notify player
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.TriggerNotification($"Level up! You have {availableSkillPoints} skill points available.");
        }
    }
    
    public bool UnlockSkill(string skillId)
    {
        if (availableSkillPoints <= 0)
        {
            Debug.LogWarning("No skill points available");
            return false;
        }
        
        if (unlockedSkills.Contains(skillId))
        {
            Debug.LogWarning($"Skill {skillId} already unlocked");
            return false;
        }
        
        if (skillTree == null)
        {
            Debug.LogError("Skill tree not found");
            return false;
        }
        
        if (!skillTree.IsNodeAvailable(skillId))
        {
            Debug.LogWarning($"Skill {skillId} is not available for unlock");
            return false;
        }
        
        // Unlock the skill
        if (skillTree.UnlockNode(skillId))
        {
            unlockedSkills.Add(skillId);
            availableSkillPoints--;
            
            // Notify player
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.TriggerNotification($"Skill unlocked: {skillTree.GetNode(skillId).Name}");
            }
            
            return true;
        }
        
        return false;
    }
    
    public int GetAvailableSkillPoints()
    {
        return availableSkillPoints;
    }
    
    public bool IsSkillUnlocked(string skillId)
    {
        return unlockedSkills.Contains(skillId);
    }
    
    public List<string> GetUnlockedSkills()
    {
        return new List<string>(unlockedSkills);
    }
    
    public void ResetSkills()
    {
        if (skillTree == null)
        {
            Debug.LogError("Skill tree not found");
            return;
        }
        
        // Calculate total skill points spent
        int pointsToRefund = unlockedSkills.Count;
        
        // Reset skill tree
        // TODO: Implement skill tree reset
        
        // Reset unlocked skills
        unlockedSkills.Clear();
        
        // Refund skill points
        availableSkillPoints += pointsToRefund;
        
        // Notify player
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.TriggerNotification($"Skills reset. You have {availableSkillPoints} skill points available.");
        }
    }
    
    public void AddSkillPoints(int amount)
    {
        availableSkillPoints += amount;
        
        // Notify player
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.TriggerNotification($"Gained {amount} skill points. Total: {availableSkillPoints}");
        }
    }
}