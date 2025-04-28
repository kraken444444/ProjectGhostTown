using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class StatBonusEntry
{
    [TableColumnWidth(130)]
    public GameEnums.StatType Stat;
    
    [TableColumnWidth(70)]
    [SuffixLabel("%", true)]
    [ProgressBar(0, 20, r: 0.1f, g: 0.8f, b: 0.4f)]
    public float Value;
    
    public StatBonusEntry(GameEnums.StatType stat, float value)
    {
        Stat = stat;
        Value = value;
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
