using UnityEngine;

// UIManager.cs - Basic UI management for the game
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Singleton instance
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = GameObject.Find("UIManager");
                if (obj == null)
                {
                    obj = new GameObject("UIManager");
                }
                
                _instance = obj.GetComponent<UIManager>() ?? obj.AddComponent<UIManager>();
            }
            return _instance;
        }
    }
    
    // UI Panels
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject skillTreePanel;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject characterCreationPanel;
    
    // UI Elements
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform notificationContainer;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider resourceBar;
    [SerializeField] private Slider experienceBar;
    [SerializeField] private Text playerLevelText;
    [SerializeField] private Text areaNameText;
    [SerializeField] private Text playTimeText;
    
    // Other UI elements
    private List<GameObject> activeNotifications = new List<GameObject>();
    private float notificationDuration = 3f;
    
    // Current state
    private bool isPaused = false;
    
    private void Awake()
    {
        // Handle singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        // Don't destroy on load if needed
        // DontDestroyOnLoad(gameObject);
        
        // Find UI elements if not assigned
        FindUIElements();
        
        // Hide all panels initially
        HideAllPanels();
        
        // Show the appropriate panel based on game state
        ShowMainMenuPanel();
    }
    
    private void FindUIElements()
    {
        // Find UI panels if not assigned
        if (mainMenuPanel == null)
            mainMenuPanel = GameObject.Find("MainMenuPanel");
            
        if (gameplayPanel == null)
            gameplayPanel = GameObject.Find("GameplayPanel");
            
        if (inventoryPanel == null)
            inventoryPanel = GameObject.Find("InventoryPanel");
            
        if (skillTreePanel == null)
            skillTreePanel = GameObject.Find("SkillTreePanel");
            
        if (dialoguePanel == null)
            dialoguePanel = GameObject.Find("DialoguePanel");
            
        if (pausePanel == null)
            pausePanel = GameObject.Find("PausePanel");
            
        if (loadingPanel == null)
            loadingPanel = GameObject.Find("LoadingPanel");
            
        if (saveLoadPanel == null)
            saveLoadPanel = GameObject.Find("SaveLoadPanel");
            
        if (characterCreationPanel == null)
            characterCreationPanel = GameObject.Find("CharacterCreationPanel");
            
        // Find UI elements if not assigned
        if (healthBar == null && gameplayPanel != null)
            healthBar = gameplayPanel.transform.Find("HealthBar")?.GetComponent<Slider>();
            
        if (resourceBar == null && gameplayPanel != null)
            resourceBar = gameplayPanel.transform.Find("ResourceBar")?.GetComponent<Slider>();
            
        if (experienceBar == null && gameplayPanel != null)
            experienceBar = gameplayPanel.transform.Find("ExperienceBar")?.GetComponent<Slider>();
            
        if (playerLevelText == null && gameplayPanel != null)
            playerLevelText = gameplayPanel.transform.Find("LevelText")?.GetComponent<Text>();
            
        if (areaNameText == null && gameplayPanel != null)
            areaNameText = gameplayPanel.transform.Find("AreaNameText")?.GetComponent<Text>();
            
        if (playTimeText == null && gameplayPanel != null)
            playTimeText = gameplayPanel.transform.Find("PlayTimeText")?.GetComponent<Text>();
            
        if (notificationContainer == null && gameplayPanel != null)
            notificationContainer = gameplayPanel.transform.Find("NotificationContainer");
    }
    
    private void OnEnable()
    {
        // Subscribe to events
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnPlayerHealthChanged += UpdateHealthBar;
            GameEvents.Instance.OnPlayerResourceChanged += UpdateResourceBar;
            GameEvents.Instance.OnPlayerExperienceGained += UpdateExperienceBar;
            GameEvents.Instance.OnPlayerLevelUp += UpdatePlayerLevel;
            GameEvents.Instance.OnAreaChanged += UpdateAreaName;
            GameEvents.Instance.OnNotification += ShowNotification;
            GameEvents.Instance.OnDialogueStarted += ShowDialogue;
            GameEvents.Instance.OnDialogueEnded += HideDialogue;
            GameEvents.Instance.OnGamePaused += PauseGame;
            GameEvents.Instance.OnGameResumed += ResumeGame;
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnPlayerHealthChanged -= UpdateHealthBar;
            GameEvents.Instance.OnPlayerResourceChanged -= UpdateResourceBar;
            GameEvents.Instance.OnPlayerExperienceGained -= UpdateExperienceBar;
            GameEvents.Instance.OnPlayerLevelUp -= UpdatePlayerLevel;
            GameEvents.Instance.OnAreaChanged -= UpdateAreaName;
            GameEvents.Instance.OnNotification -= ShowNotification;
            GameEvents.Instance.OnDialogueStarted -= ShowDialogue;
            GameEvents.Instance.OnDialogueEnded -= HideDialogue;
            GameEvents.Instance.OnGamePaused -= PauseGame;
            GameEvents.Instance.OnGameResumed -= ResumeGame;
        }
    }
    
    private void Update()
    {
        // Update play time
        if (gameplayPanel != null && gameplayPanel.activeInHierarchy && playTimeText != null)
        {
            if (GameState.Instance != null)
            {
                float playTime = GameState.Instance.PlayTime;
                int hours = Mathf.FloorToInt(playTime / 3600);
                int minutes = Mathf.FloorToInt((playTime % 3600) / 60);
                int seconds = Mathf.FloorToInt(playTime % 60);
                
                playTimeText.text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
            }
        }
        
        // Handle pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    // Panel management
    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (skillTreePanel != null) skillTreePanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
        if (characterCreationPanel != null) characterCreationPanel.SetActive(false);
    }
    
    public void ShowMainMenuPanel()
    {
        HideAllPanels();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
    
    public void ShowGameplayPanel()
    {
        HideAllPanels();
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
        
        // Update UI elements
        UpdatePlayerUI();
    }
    
    public void ShowInventoryPanel()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
    }
    
    public void HideInventoryPanel()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }
    
    public void ShowSkillTreePanel()
    {
        if (skillTreePanel != null) skillTreePanel.SetActive(true);
    }
    
    public void HideSkillTreePanel()
    {
        if (skillTreePanel != null) skillTreePanel.SetActive(false);
    }
    
    private void ShowDialogue(string npcName, string dialogueId)
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        
        // TODO: Set up dialogue UI
    }
    
    private void HideDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }
    
    public void ShowLoadingPanel()
    {
        HideAllPanels();
        if (loadingPanel != null) loadingPanel.SetActive(true);
    }
    
    public void ShowSaveLoadPanel()
    {
        if (saveLoadPanel != null) saveLoadPanel.SetActive(true);
        
        // TODO: Populate save/load UI
    }
    
    public void HideSaveLoadPanel()
    {
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
    }
    
    public void ShowCharacterCreationPanel()
    {
        HideAllPanels();
        if (characterCreationPanel != null) characterCreationPanel.SetActive(true);
    }
    
    // UI update methods
    private void UpdatePlayerUI()
    {
        if (GameState.Instance != null && GameState.Instance.PlayerCharacter != null)
        {
            PlayerCharacter player = GameState.Instance.PlayerCharacter;
            
            // Update health bar
            if (healthBar != null)
            {
                healthBar.maxValue = player.maxHealth;
                healthBar.value = player.currentHealth;
            }
            
            // Update resource bar
            if (resourceBar != null)
            {
                resourceBar.maxValue = player.maxResource;
                resourceBar.value = player.currentResource;
            }
            
            // Update level text
            if (playerLevelText != null)
            {
                playerLevelText.text = $"Level: {player.level}";
            }
            
            // Update experience bar
            if (experienceBar != null)
            {
                experienceBar.maxValue = player.experienceToNextLevel;
                experienceBar.value = player.experience;
            }
            
            // Update area name
            if (areaNameText != null && GameState.Instance.CurrentAreaID != null)
            {
                areaNameText.text = GameState.Instance.CurrentAreaID;
            }
        }
    }
    
    private void UpdateHealthBar(int previousHealth, int currentHealth)
    {
        if (healthBar != null && GameState.Instance != null && GameState.Instance.PlayerCharacter != null)
        {
            healthBar.maxValue = GameState.Instance.PlayerCharacter.maxHealth;
            healthBar.value = currentHealth;
        }
    }
    
    private void UpdateResourceBar(int previousResource, int currentResource)
    {
        if (resourceBar != null && GameState.Instance != null && GameState.Instance.PlayerCharacter != null)
        {
            resourceBar.maxValue = GameState.Instance.PlayerCharacter.maxResource;
            resourceBar.value = currentResource;
        }
    }
    
    private void UpdateExperienceBar(int experienceGained)
    {
        if (experienceBar != null && GameState.Instance != null && GameState.Instance.PlayerCharacter != null)
        {
            experienceBar.maxValue = GameState.Instance.PlayerCharacter.experienceToNextLevel;
            experienceBar.value = GameState.Instance.PlayerCharacter.experience;
        }
    }
    
    private void UpdatePlayerLevel(int newLevel)
    {
        if (playerLevelText != null)
        {
            playerLevelText.text = $"Level: {newLevel}";
        }
    }
    
    private void UpdateAreaName(string areaId)
    {
        if (areaNameText != null)
        {
            areaNameText.text = areaId;
        }
    }
    
    // Notification system
    public void ShowNotification(string message)
    {
        if (notificationContainer == null || notificationPrefab == null)
        {
            Debug.LogWarning("Notification container or prefab not assigned");
            return;
        }
        
        // Create notification
        GameObject notification = Instantiate(notificationPrefab, notificationContainer);
        
        // Set message
        Text notificationText = notification.GetComponentInChildren<Text>();
        if (notificationText != null)
        {
            notificationText.text = message;
        }
        
        // Add to active notifications
        activeNotifications.Add(notification);
        
        // Destroy after duration
        Destroy(notification, notificationDuration);
        
        // Remove from list after duration
        Invoke("RemoveNotification", notificationDuration);
    }
    
    private void RemoveNotification()
    {
        if (activeNotifications.Count > 0)
        {
            GameObject notification = activeNotifications[0];
            activeNotifications.RemoveAt(0);
        }
    }
    
    // Pause system
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    private void PauseGame()
    {
        isPaused = true;
        
        // Show pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        
        // Pause game time
        Time.timeScale = 0f;
    }
    
    private void ResumeGame()
    {
        isPaused = false;
        
        // Hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        // Resume game time
        Time.timeScale = 1f;
    }
}