using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapControllerManual : MonoBehaviour
{
    public static MapControllerManual Instance { get; private set; }

    public GameObject mapParent;
    private List<Image> _mapImages;
    
    public Color highlightColour = Color.yellow;
    public Color dimmedColour = new Color(1f, 1f, 1f, 0.5f);
    
    public RectTransform playerIconTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        _mapImages = mapParent.GetComponentsInChildren<Image>().ToList();
    }

    public void HighlightArea(string areaName)
    {
        foreach (Image area in _mapImages)
        {
            area.color = dimmedColour;
        }
        
        Image currentArea = _mapImages.Find(x => x.name == areaName);

        if (currentArea != null)
        {
            currentArea.color = highlightColour;
            playerIconTransform.position = currentArea.GetComponent<RectTransform>().position;
        }
        else
        {
            Debug.LogWarning("Area not found " + areaName);
        }
    }
}
