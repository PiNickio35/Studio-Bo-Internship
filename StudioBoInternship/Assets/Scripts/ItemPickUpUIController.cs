using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUpUIController : MonoBehaviour
{
    public static ItemPickUpUIController Instance { get; private set; }
    
    public GameObject popUpPrefab;
    public int maxPopUps = 5;
    public float popUpDuration = 3f;

    private readonly Queue<GameObject> activePopUps = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowItemPickUp(string itemName, Sprite itemIcon)
    {
        GameObject newPopUp = Instantiate(popUpPrefab, transform);
        newPopUp.GetComponentInChildren<TMP_Text>().text = itemName;
        
        Image itemImage = newPopUp.transform.Find("ItemIcon")?.GetComponent<Image>();
        if (itemImage)
        {
            itemImage.sprite = itemIcon;
        }
        
        activePopUps.Enqueue(newPopUp);
        if (activePopUps.Count > maxPopUps)
        {
            Destroy(activePopUps.Dequeue());
        }
        
        StartCoroutine(FadeOutAndDestroy(newPopUp));
    }

    private IEnumerator FadeOutAndDestroy(GameObject popUp)
    {
        yield return new WaitForSeconds(popUpDuration);
        if (popUp == null) yield break;
        
        CanvasGroup canvasGroup = popUp.GetComponent<CanvasGroup>();
        for (float timePassed = 0; timePassed < 1f; timePassed += Time.deltaTime)
        {
            if (popUp == null) yield break;
            canvasGroup.alpha = 1f - timePassed;
            yield return null;
        }
        
        Destroy(popUp);
    }
}
