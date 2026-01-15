using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    private string _saveLocation;
    private InventoryController _inventoryController;
    private HotbarController _hotbarController;
    private Chest[] chests;

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
        chests = FindObjectsByType<Chest>(FindObjectsSortMode.None);
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
            hotbarSaveData = _hotbarController.GetHotbarItems(),
            chestSaveData = GetChestStates()
        };
        
        File.WriteAllText(_saveLocation, JsonUtility.ToJson(saveData));
    }

    private List<ChestSaveData> GetChestStates()
    {
        List<ChestSaveData> chestStates = new List<ChestSaveData>();

        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = new ChestSaveData
            {
                chestID = chest.chestID,
                isOpened = chest.isOpened
            };
            chestStates.Add(chestSaveData);
        }
        
        return chestStates;
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
            LoadChestStates(saveData.chestSaveData);
        }
        else
        {
            SaveGame();
            
            _inventoryController.SetInventoryItems(new List<InventorySaveData>());
            _hotbarController.SetHotbarItems(new List<InventorySaveData>());
        }
    }

    private void LoadChestStates(List<ChestSaveData> chestStates)
    {
        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = chestStates.FirstOrDefault(c => c.chestID == chest.chestID);

            if (chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.isOpened);
            }
        }
    }
}
