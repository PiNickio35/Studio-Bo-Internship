using System;
using System.Collections.Generic;
using System.IO;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    private string _saveLocation;
    private InventoryController _inventoryController;
    private HotbarController _hotbarController;

    private void Awake()
    {
        _saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _inventoryController = FindFirstObjectByType<InventoryController>();
        _hotbarController = FindFirstObjectByType<HotbarController>();
    }

    private void Start()
    {
        _inventoryController = FindFirstObjectByType<InventoryController>();
        _hotbarController = FindFirstObjectByType<HotbarController>();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundary = FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D.gameObject.name,
            inventorySaveData = _inventoryController.GetInventoryItems(),
            hotbarSaveData = _hotbarController.GetHotbarItems()
        };
        
        File.WriteAllText(_saveLocation, JsonUtility.ToJson(saveData));
    }
    
    public void LoadGame()
    {
        if (File.Exists(_saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(_saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
            FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();
            MapControllerManual.Instance?.HighlightArea(saveData.mapBoundary);
            _inventoryController.SetInventoryItems(saveData.inventorySaveData);
            _hotbarController.SetHotbarItems(saveData.hotbarSaveData);
        }
        else
        {
            SaveGame();
            
            _inventoryController.SetInventoryItems(new List<InventorySaveData>());
            _hotbarController.SetHotbarItems(new List<InventorySaveData>());
        }
    }
}
