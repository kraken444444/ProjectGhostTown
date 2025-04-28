using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Sirenix.Utilities.Editor;
using Unity.VisualScripting;
using UnityEngine;

// Component to display character class information
public class CharacterClassDisplay : SerializedMonoBehaviour
{
    [SerializeField] private CharacterManager manager;
    
    // Toggle for showing class details
    [SerializeField, PropertyOrder(-100)]
    [LabelText("Show Class Details")]
    [OnValueChanged("UpdateClassDisplay")]
    [GUIColor(0.3f, 0.8f, 0.5f)]
    private bool showClassDetails = false;
    
    // Reference to the character class (updated in OnEnable)
    [ShowIf("showClassDetails")]
    [BoxGroup("Class Details/Basic Info", showLabel: false)]
    [TitleGroup("Class Details/Basic Info/Header", "$className")]
    [ShowInInspector, ReadOnly, PropertyOrder(-10)]
    [EnumPaging, GUIColor(0.6f, 0.8f, 1f)]
    private GameEnums.ClassType classType;
    
    [ShowIf("showClassDetails")]
    [BoxGroup("Class Details/Basic Info")]
    [ShowInInspector, ReadOnly, PropertyOrder(-9)]
    [EnumPaging, GUIColor(0.6f, 0.9f, 0.6f)]
    private GameEnums.ResourceType resourceType;
    
    [ShowIf("showClassDetails")]
    [BoxGroup("Class Details/Basic Info")]
    [ShowInInspector, ReadOnly, PropertyOrder(-8)]
    [TextArea(3, 5), HideLabel]
    private string description;
    
    // Attributes section
    [ShowIf("showClassDetails")]
    [TabGroup("Class Details/Tabs", "Attributes")]
    [ShowInInspector, ReadOnly]
    [TableList(ShowIndexLabels = true, IsReadOnly = true)]
    [LabelText("Base Attribute Modifiers")]
    private List<AttributeModifierEntry> baseModifiers = new List<AttributeModifierEntry>();
    
    [ShowIf("showClassDetails")]
    [TabGroup("Class Details/Tabs", "Attributes")]
    [ShowInInspector, ReadOnly]
    [TableList(ShowIndexLabels = true, IsReadOnly = true)]
    [LabelText("Favored Attributes")]
    private List<AttributeModifierEntry> favoredAttributes = new List<AttributeModifierEntry>();
    
    // Bonuses section
    [ShowIf("showClassDetails")]
    [TabGroup("Class Details/Tabs", "Bonuses")]
    [ShowInInspector, ReadOnly]
    [TableList(ShowIndexLabels = true, IsReadOnly = true)]
    private List<StatBonusEntry> classBonuses = new List<StatBonusEntry>();
    
    // Subclasses section
    [ShowIf("showClassDetails && HasSubclasses")]
    [TabGroup("Class Details/Tabs", "Subclasses")]
    [ShowInInspector, ReadOnly]
    [ListDrawerSettings(ShowFoldout = true, ShowPaging = true, ShowItemCount = true)]
    private List<Subclass> availableSubclasses = new List<Subclass>();
    
    // Starting cards section
    [ShowIf("showClassDetails && HasStartingCards")]
    [TabGroup("Class Details/Tabs", "Starting Cards")]
    [ShowInInspector, ReadOnly]
    [ListDrawerSettings(ShowFoldout = true, ShowPaging = true, ShowItemCount = true)]
    private List<CardTemplate> startingCards = new List<CardTemplate>();
    
    private string className = "Character Class";
    
    private bool HasSubclasses => manager?.PlayerCharacter?.Class?.GetAvailableSubclasses() != null && manager.PlayerCharacter.Class.GetAvailableSubclasses().Count > 0;
    private bool HasStartingCards => manager?.PlayerCharacter?.Class?.StartingCards != null && manager.PlayerCharacter.Class.StartingCards.Count > 0;
    
    [Button("Toggle Class Info"), GUIColor(0.3f, 0.6f, 0.9f), PropertyOrder(-99)]
    private void ToggleClassDetails()
    {
        showClassDetails = !showClassDetails;
        UpdateClassDisplay();
    }
    
    private void UpdateClassDisplay()
    {
        if (manager == null || manager.PlayerCharacter == null || manager.PlayerCharacter.Class == null)
            return;
        
        CharacterClass characterClass = manager.PlayerCharacter.Class;
        
        //basic info
        className = characterClass.Name;
        classType = characterClass.Type;
        resourceType = characterClass.ResourceType;
        description = characterClass.Description;
        
        // update attributes
        baseModifiers = ConvertToList(characterClass.BaseAttributeModifiers);
        favoredAttributes = ConvertToList(characterClass.FavoredAttributes);
        
        //update bonuses
        classBonuses = ConvertToStatBonusList(characterClass.ClassBonuses);
        
        //update collections
        availableSubclasses = characterClass.GetAvailableSubclasses();
        startingCards = characterClass.StartingCards;
        
        //force inspector update
        GUIHelper.RequestRepaint();
    }
    
    private void OnEnable()
    {
        UpdateClassDisplay();
    }
    
    private List<AttributeModifierEntry> ConvertToList(Dictionary<GameEnums.AttributeType, int> dictionary)
    {
        List<AttributeModifierEntry> result = new List<AttributeModifierEntry>();
        if (dictionary != null)
        {
            foreach (var kvp in dictionary)
            {
                result.Add(new AttributeModifierEntry(kvp.Key, kvp.Value));
            }
        }
        return result;
    }

    private List<StatBonusEntry> ConvertToStatBonusList(Dictionary<GameEnums.StatType, float> dictionary)
    {
        List<StatBonusEntry> result = new List<StatBonusEntry>();
        if (dictionary != null)
        {
            foreach (var kvp in dictionary)
            {
                result.Add(new StatBonusEntry(kvp.Key, kvp.Value));
            }
        }
        return result;
    }
}