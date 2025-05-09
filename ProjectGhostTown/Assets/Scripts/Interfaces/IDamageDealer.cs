using System.Collections.Generic;
using UnityEngine;

public interface IDamageDealer
{
    string ID { get; }
    string Name { get; }

    Dictionary<GameEnums.AttributeType, int> GetAttributes();

    float GetStat(GameEnums.StatType statType);
    int GetAttributeValue(GameEnums.AttributeType attributeType);
    float GetStatValue(GameEnums.StatType statType);
}

