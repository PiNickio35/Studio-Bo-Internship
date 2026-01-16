using System;
using Interfaces;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool isOpened { get; private set; }
    public string chestID { get; private set; }
    public GameObject itemPrefab;
    public Sprite openedSprite;

    private void Start()
    {
        chestID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        OpenChest();
    }

    public bool CanInteract()
    {
        return !isOpened;
    }

    private void OpenChest()
    {
        SetOpened(true);
        SoundEffectManager.Play("Chest");
        if (itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
            droppedItem.GetComponent<BounceEffect>().StartBounce();
        }
    }

    public void SetOpened(bool opened)
    {
        isOpened = opened;
        if (isOpened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }
}
