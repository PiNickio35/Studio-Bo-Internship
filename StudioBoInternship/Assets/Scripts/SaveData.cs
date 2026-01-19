using System.Collections.Generic;
using Base_Classes;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public string mapBoundary;
    public float sliderVolume;
    public List<InventorySaveData> inventorySaveData;
    public List<InventorySaveData> hotbarSaveData;
    public List<ChestSaveData> chestSaveData;
    public List<BaseHero> heroSaveData;
}

[System.Serializable]
public class ChestSaveData
{
    public string chestID;
    public bool isOpened;
}
