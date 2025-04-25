[System.Serializable]
public class CardTemplate
{
    public string ID;
    public string Name;
    public string Description;
    public CardRarity Rarity;
    
    public CardTemplate(string name, string description, CardRarity rarity)
    {
        ID = name.Replace(" ", "");
        Name = name;
        Description = description;
        Rarity = rarity;
    }
    
    public Card CreateCardInstance()
    {
        return new Card(this);
    }
}

public enum CardRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary,
    Unique
}