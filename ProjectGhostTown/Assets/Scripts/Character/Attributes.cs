using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attributes
{
    [SerializeField] // Make it serializable in Unity's inspector
    private Dictionary<GameEnums.AttributeType, int> _values = new Dictionary<GameEnums.AttributeType, int>();
    
    // Events for attribute changes
    public event Action<GameEnums.AttributeType, int, int> OnAttributeChanged; // type, old, new
    
    public Attributes()
    {
        // Initialize all attributes with default values
        foreach (GameEnums.AttributeType type in Enum.GetValues(typeof(GameEnums.AttributeType)))
        {
            _values[type] = 0;
        }
    }
    
    // Copy constructor
    public Attributes(Attributes other)
    {
        foreach (var kvp in other._values)
        {
            _values[kvp.Key] = kvp.Value;
        }
    }
    
    // Initialize with specific values
    public Attributes(Dictionary<GameEnums.AttributeType, int> initialValues)
    {
        // First set all to zero
        foreach (GameEnums.AttributeType type in Enum.GetValues(typeof(GameEnums.AttributeType)))
        {
            _values[type] = 0;
        }
        
        // Then override with provided values
        foreach (var kvp in initialValues)
        {
            _values[kvp.Key] = kvp.Value;
        }
    }
    
    // Indexer for easy access
    public int this[GameEnums.AttributeType type]
    {
        get => GetValue(type);
        set => SetValue(type, value);
    }
    
    public int GetValue(GameEnums.AttributeType type)
    {
        return _values.ContainsKey(type) ? _values[type] : 0;
    }
    
    public void SetValue(GameEnums.AttributeType type, int value)
    {
        int oldValue = GetValue(type);
        _values[type] = value;
        
        OnAttributeChanged?.Invoke(type, oldValue, value);
    }
    
    public void ModifyValue(GameEnums.AttributeType type, int amount)
    {
        SetValue(type, GetValue(type) + amount);
    }
    
    //modifiers from class, items, etc.
    public void ApplyModifiers(Dictionary<GameEnums.AttributeType, int> modifiers)
    {
        foreach (var modifier in modifiers)
        {
            ModifyValue(modifier.Key, modifier.Value);
        }
    }
    
    //get all values as a dictionary (copy to prevent direct modification)
    public Dictionary<GameEnums.AttributeType, int> GetAllValues()
    {
        return new Dictionary<GameEnums.AttributeType, int>(_values);
    }
    
    //reset attribute to zero
    public void Reset(GameEnums.AttributeType type)
    {
        SetValue(type, 0);
    }
    
    // Reset all attributes to zero
    public void ResetAll()
    {
        foreach (GameEnums.AttributeType type in Enum.GetValues(typeof(GameEnums.AttributeType)))
        {
            Reset(type);
        }
    }
}