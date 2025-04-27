using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[Serializable]

public class AttributeModifierEntry
{
    [TableColumnWidth(120)]
    public AttributeType Attribute;
    
    [TableColumnWidth(80)]
    [ProgressBar(0, 10, r: 0.2f, g: 0.6f, b: 1f)]
    public int Value;
    
    public AttributeModifierEntry(AttributeType attribute, int value)
    {
        Attribute = attribute;
        Value = value;
    }
    private List<AttributeModifierEntry> ConvertToList(Dictionary<AttributeType, int> dictionary)
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
}