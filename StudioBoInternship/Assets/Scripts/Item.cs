using UnityEngine;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;

    public virtual void UseItem()
    {
        Debug.Log("Using item " + Name);
    }

    public virtual void Pickup()
    {
        Sprite itemIcon = GetComponent<SpriteRenderer>().sprite;
        if (ItemPickUpUIController.Instance != null)
        {
            ItemPickUpUIController.Instance.ShowItemPickUp(Name, itemIcon);
        }
    }
}
