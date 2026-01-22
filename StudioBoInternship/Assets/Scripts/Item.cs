using Base_Classes;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemID;
    public string itemName;
    public enum ItemType
    {
        Potion,
        Ether
    }
    public ItemType itemType;

    public void UseItem(int characterChoice)
    {
        BaseHero selectedCharacter = GameManager.Instance.updatedHeroes[characterChoice];
        switch (itemType)
        {
            case ItemType.Potion:
                selectedCharacter.CurrentHp = Mathf.Min(selectedCharacter.BaseHp, selectedCharacter.CurrentHp + 100);
                break;
            case ItemType.Ether:
                selectedCharacter.CurrentMp = Mathf.Min(selectedCharacter.BaseMp, selectedCharacter.CurrentMp + 20);
                break;
            default:
                break;
        }
        Destroy(gameObject);
    }

    public void Pickup()
    {
        Sprite itemIcon = GetComponent<SpriteRenderer>().sprite;
        if (ItemPickUpUIController.Instance != null)
        {
            ItemPickUpUIController.Instance.ShowItemPickUp(itemName, itemIcon);
        }
    }
}
