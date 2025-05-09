using System.Collections.Generic;

[System.Serializable]
public class Subclass
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public CharacterClass ParentClass { get; private set; }

    public Dictionary<AttributeType, int> AttributeModifiers { get; private set; }


    public Subclass(
        string name,
        string description,
        CharacterClass parentClass,
        Dictionary<AttributeType, int> attributeModifier)
    {
        Name = name;
        Description = description;
        ParentClass = parentClass;
    }
}