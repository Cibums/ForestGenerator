using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    public Item item = null;
    public bool pickupable = true;

    private void Update()
    {
        if (Vector3.Distance(GameController.ins.player.position, transform.position) > GameController.ins.renderDistance)
        {
            ItemMetaData meta = new ItemMetaData();
            meta.itemID = GameController.ins.GetItemID(item);
            meta.position = transform.position;
            meta.rotation = transform.rotation;

            GameController.ins.loadedItems.Remove(transform);
            GameController.ins.unloadedItems.Add(meta);

            Destroy(gameObject);
        }
    }

    public void Pickup()
    {
        if (!pickupable)
        {
            return;
        }

        AudioController.PlaySound(0);
        GameController.ins.loadedItems.Remove(transform);
        Inventory.Add(item);
        Destroy(gameObject);
    }
}
