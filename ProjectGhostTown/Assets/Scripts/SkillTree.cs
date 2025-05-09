using UnityEngine;

// SkillTree.cs
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    [System.Serializable]
    public class SkillNode
    {
        public string ID;
        public string Name;
        public string Description;
        public int LevelRequirement;
        public int SkillPointCost;
        public List<string> Prerequisites = new List<string>();
        public GameEnums.ClassType ClassRequirement;
        public Dictionary<GameEnums.AttributeType, int> AttributeModifiers = new Dictionary<GameEnums.AttributeType, int>();
        public Dictionary<GameEnums.StatType, float> StatModifiers = new Dictionary<GameEnums.StatType, float>();
        public List<string> UnlockedCards = new List<string>();
        public Sprite Icon;
        public Vector2 Position;
        
        // For runtime use
        [HideInInspector] public bool IsUnlocked;
        [HideInInspector] public bool IsAvailable;
    }
    
    // Serialized node data
    [SerializeField] private List<SkillNode> availableNodes = new List<SkillNode>();
    
    // Runtime tracking
    private Dictionary<string, SkillNode> nodesById = new Dictionary<string, SkillNode>();
    private HashSet<string> unlockedNodes = new HashSet<string>();
    private PlayerCharacter playerCharacter;
    
    // Events
    public System.Action<SkillNode> OnNodeUnlocked;
    
    private void Awake()
    {
        // Build the node lookup dictionary
        foreach (var node in availableNodes)
        {
            if (!string.IsNullOrEmpty(node.ID))
            {
                nodesById[node.ID] = node;
            }
            else
            {
                Debug.LogError("SkillTree has a node with empty ID");
            }
        }
    }
    
    public void Initialize(PlayerCharacter character)
    {
        playerCharacter = character;
        
        // Reset all nodes
        unlockedNodes.Clear();
        
        foreach (var node in availableNodes)
        {
            node.IsUnlocked = false;
            node.IsAvailable = false;
        }
        
        // Update node availability
        UpdateNodeAvailability();
    }
    
    public bool UnlockNode(string nodeId)
    {
        if (!nodesById.TryGetValue(nodeId, out SkillNode node))
        {
            Debug.LogError($"Tried to unlock non-existent node: {nodeId}");
            return false;
        }
        
        if (node.IsUnlocked)
        {
            Debug.Log($"Node {node.Name} is already unlocked");
            return false;
        }
        
        if (!node.IsAvailable)
        {
            Debug.LogError($"Cannot unlock node {node.Name} - prerequisites not met");
            return false;
        }
        
        // Check level requirement
        if (playerCharacter.GetCharacterClass().Type != node.ClassRequirement)
        {
            Debug.LogError($"Cannot unlock node {node.Name} - wrong character class");
            return false;
        }
        
        // TODO: Check skill points
        
        // Unlock the node
        node.IsUnlocked = true;
        unlockedNodes.Add(nodeId);
        
        // Apply attribute and stat modifiers
        if (playerCharacter != null)
        {
            foreach (var modifier in node.AttributeModifiers)
            {
                playerCharacter.ModifyAttribute(modifier.Key, modifier.Value);
            }
            
            // TODO: Apply stat modifiers
        }
        
        // Unlock cards
        if (node.UnlockedCards.Count > 0)
        {
            // TODO: Create and add cards to the player deck
        }
        
        // Update availability
        UpdateNodeAvailability();
        
        // Trigger event
        OnNodeUnlocked?.Invoke(node);
        
        Debug.Log($"Unlocked skill node: {node.Name}");
        return true;
    }
    
    private void UpdateNodeAvailability()
    {
        foreach (var node in availableNodes)
        {
            if (node.IsUnlocked)
            {
                node.IsAvailable = true;
                continue;
            }
            
            // Check if all prerequisites are unlocked
            bool prereqsMet = true;
            foreach (var prereqId in node.Prerequisites)
            {
                if (!unlockedNodes.Contains(prereqId))
                {
                    prereqsMet = false;
                    break;
                }
            }
            
            // Check level requirement
            bool levelMet = false;
            if (playerCharacter != null)
            {
                // TODO: Check player level vs node level requirement
                levelMet = true;
            }
            
            // Check class requirement
            bool classMet = false;
            if (playerCharacter != null && playerCharacter.GetCharacterClass() != null)
            {
                classMet = (node.ClassRequirement == GameEnums.ClassType.Radiomancer) || 
                           (playerCharacter.GetCharacterClass().Type == node.ClassRequirement);
            }
            
            node.IsAvailable = prereqsMet && levelMet && classMet;
        }
    }
    
    public List<SkillNode> GetAvailableNodes()
    {
        List<SkillNode> availableNodes = new List<SkillNode>();
        
        foreach (var node in this.availableNodes)
        {
            if (node.IsAvailable && !node.IsUnlocked)
            {
                availableNodes.Add(node);
            }
        }
        
        return availableNodes;
    }
    
    public List<SkillNode> GetUnlockedNodes()
    {
        List<SkillNode> unlockedNodes = new List<SkillNode>();
        
        foreach (var node in this.availableNodes)
        {
            if (node.IsUnlocked)
            {
                unlockedNodes.Add(node);
            }
        }
        
        return unlockedNodes;
    }
    
    public SkillNode GetNode(string nodeId)
    {
        if (nodesById.TryGetValue(nodeId, out SkillNode node))
        {
            return node;
        }
        
        return null;
    }
    
    public bool IsNodeUnlocked(string nodeId)
    {
        return unlockedNodes.Contains(nodeId);
    }
    
    public bool IsNodeAvailable(string nodeId)
    {
        if (nodesById.TryGetValue(nodeId, out SkillNode node))
        {
            return node.IsAvailable && !node.IsUnlocked;
        }
        
        return false;
    }
}