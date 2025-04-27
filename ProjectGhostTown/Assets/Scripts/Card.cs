using UnityEngine;

[System.Serializable]
public class Card
{
    public string ID;
    public CardTemplate Template;

    public Card(CardTemplate template)
    {
        Template = template;
        ID = System.Guid.NewGuid().ToString();
    }
}
    

