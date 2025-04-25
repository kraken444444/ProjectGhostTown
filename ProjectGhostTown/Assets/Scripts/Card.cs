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
    
    public void ApplyEffects(Character user, Character target)
    {
        // Basic implementation
        Debug.Log(user + " used " + Template.Name + " on " + target);
    }
}
