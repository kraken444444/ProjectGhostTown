using UnityEngine;

// SaveLoadManager.cs - Handles saving and loading game state
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private const string SAVE_EXTENSION = ".save";
    private const string SAVE_DIRECTORY = "/Saves/";
    
    // Singleton instance
    private static SaveLoadManager _instance;
    public static SaveLoadManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("SaveLoadManager");
                _instance = obj.AddComponent<SaveLoadManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
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
        
        // Create save directory if it doesn't exist
        string saveDir = Application.persistentDataPath + SAVE_DIRECTORY;
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }
    }
    
    public void SaveGame(string saveName)
    {
        try
        {
            // Get save data from game state
            SaveData saveData = GameState.Instance.CreateSaveData();
            
            // Convert to JSON
            string json = JsonUtility.ToJson(saveData, true);
            
            // Write to file
            string savePath = GetSavePath(saveName);
            File.WriteAllText(savePath, json);
            
            Debug.Log($"Game saved successfully to {savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}\n{e.StackTrace}");
        }
    }
    
    public bool LoadGame(string saveName)
    {
        try
        {
            string savePath = GetSavePath(saveName);
            
            // Check if save exists
            if (!File.Exists(savePath))
            {
                Debug.LogWarning($"Save file not found: {savePath}");
                return false;
            }
            
            // Read JSON from file
            string json = File.ReadAllText(savePath);
            
            // Convert to save data
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            
            // Load into game state
            GameState.Instance.LoadGame(saveData);
            
            Debug.Log($"Game loaded successfully from {savePath}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading game: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }
    
    public bool DeleteSave(string saveName)
    {
        try
        {
            string savePath = GetSavePath(saveName);
            
            // Check if save exists
            if (!File.Exists(savePath))
            {
                Debug.LogWarning($"Save file not found: {savePath}");
                return false;
            }
            
            // Delete file
            File.Delete(savePath);
            
            Debug.Log($"Save file deleted: {savePath}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error deleting save: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }
    
    public List<string> GetAllSaves()
    {
        try
        {
            string saveDir = Application.persistentDataPath + SAVE_DIRECTORY;
            
            // Check if directory exists
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
                return new List<string>();
            }
            
            // Get all save files
            string[] files = Directory.GetFiles(saveDir, "*" + SAVE_EXTENSION);
            
            // Extract save names
            List<string> saveNames = new List<string>();
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                saveNames.Add(fileName);
            }
            
            return saveNames;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting saves: {e.Message}\n{e.StackTrace}");
            return new List<string>();
        }
    }
    
    public bool DoesSaveExist(string saveName)
    {
        string savePath = GetSavePath(saveName);
        return File.Exists(savePath);
    }
    
    private string GetSavePath(string saveName)
    {
        return Application.persistentDataPath + SAVE_DIRECTORY + saveName + SAVE_EXTENSION;
    }
}