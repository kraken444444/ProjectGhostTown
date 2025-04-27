using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    private List<Card> playerDeck;




    List<Card> GetPlayerDeck()
    {
        return playerDeck;
    }
    void SetCardInDeck(List<Card> cards, int cardIndex, Card card)
    {
        cards[cardIndex] = card;
    }

    Card FindCardInDeck(Card card)
    {
        for (int i = 0; i < GetPlayerDeck().Count; i++)
        {
            if (GetPlayerDeck()[i] == card)
            {
                return GetPlayerDeck()[i];
            }
        }
        Debug.Log("Card not found in player Deck");
        return null;
    }
}
