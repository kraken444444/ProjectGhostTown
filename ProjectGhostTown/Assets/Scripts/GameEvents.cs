// GameEvents.cs - Centralized event system for game events
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    // Singleton instance
    private static GameEvents _instance;
    public static GameEvents Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("GameEvents");
                _instance = obj.AddComponent<GameEvents>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    // Player events
    public event Action<int> OnPlayerLevelUp;
    public event Action<int, int> OnPlayerHealthChanged;
    public event Action<int, int> OnPlayerResourceChanged;
    public event Action<int> OnPlayerExperienceGained;
    public event Action OnPlayerDeath;
    
    // Card events
    public event Action<Card> OnCardAdded;
    public event Action<Card> OnCardRemoved;
    public event Action<Card> OnCardPlayed;
    
    // Combat events
    public event Action<BaseCharacter> OnEnemyDefeated;
    public event Action<DamageInfo> OnDamageDealt;
    public event Action<int> OnHealingDone;
    
    // Game state events
    public event Action<string> OnAreaDiscovered;
    public event Action<string> OnAreaChanged;
    public event Action<string> OnBossDefeated;
    public event Action<string> OnMonolithActivated;
    public event Action<string> OnObjectiveCompleted;
    
    // UI events
    public event Action<string> OnNotification;
    public event Action<string, string> OnDialogueStarted;
    public event Action OnDialogueEnded;
    
    // Game system events
    public event Action OnGameSaved;
    public event Action OnGameLoaded;
    public event Action OnGameStarted;
    public event Action OnGamePaused;
    public event Action OnGameResumed;
    
    private void Awake()
    {
        // Handle singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    // Player event triggers
    public void TriggerPlayerLevelUp(int newLevel)
    {
        OnPlayerLevelUp?.Invoke(newLevel);
    }
    
    public void TriggerPlayerHealthChanged(int previousHealth, int currentHealth)
    {
        OnPlayerHealthChanged?.Invoke(previousHealth, currentHealth);
    }
    
    public void TriggerPlayerResourceChanged(int previousResource, int currentResource)
    {
        OnPlayerResourceChanged?.Invoke(previousResource, currentResource);
    }
    
    public void TriggerPlayerExperienceGained(int amount)
    {
        OnPlayerExperienceGained?.Invoke(amount);
    }
    
    public void TriggerPlayerDeath()
    {
        OnPlayerDeath?.Invoke();
    }
    
    // Card event triggers
    public void TriggerCardAdded(Card card)
    {
        OnCardAdded?.Invoke(card);
    }
    
    public void TriggerCardRemoved(Card card)
    {
        OnCardRemoved?.Invoke(card);
    }
    
    public void TriggerCardPlayed(Card card)
    {
        OnCardPlayed?.Invoke(card);
    }
    
    // Combat event triggers
    public void TriggerEnemyDefeated(BaseCharacter enemy)
    {
        OnEnemyDefeated?.Invoke(enemy);
    }
    
    public void TriggerDamageDealt(DamageInfo damageInfo)
    {
        OnDamageDealt?.Invoke(damageInfo);
    }
    
    public void TriggerHealingDone(int amount)
    {
        OnHealingDone?.Invoke(amount);
    }
    
    // Game state event triggers
    public void TriggerAreaDiscovered(string areaId)
    {
        OnAreaDiscovered?.Invoke(areaId);
    }
    
    public void TriggerAreaChanged(string areaId)
    {
        OnAreaChanged?.Invoke(areaId);
    }
    
    public void TriggerBossDefeated(string bossId)
    {
        OnBossDefeated?.Invoke(bossId);
    }
    
    public void TriggerMonolithActivated(string monolithId)
    {
        OnMonolithActivated?.Invoke(monolithId);
    }
    
    public void TriggerObjectiveCompleted(string objectiveId)
    {
        OnObjectiveCompleted?.Invoke(objectiveId);
    }
    
    // UI event triggers
    public void TriggerNotification(string message)
    {
        OnNotification?.Invoke(message);
    }
    
    public void TriggerDialogueStarted(string npcName, string dialogueId)
    {
        OnDialogueStarted?.Invoke(npcName, dialogueId);
    }
    
    public void TriggerDialogueEnded()
    {
        OnDialogueEnded?.Invoke();
    }
    
    // Game system event triggers
    public void TriggerGameSaved()
    {
        OnGameSaved?.Invoke();
    }
    
    public void TriggerGameLoaded()
    {
        OnGameLoaded?.Invoke();
    }
    
    public void TriggerGameStarted()
    {
        OnGameStarted?.Invoke();
    }
    
    public void TriggerGamePaused()
    {
        OnGamePaused?.Invoke();
    }
    
    public void TriggerGameResumed()
    {
        OnGameResumed?.Invoke();
    }
}