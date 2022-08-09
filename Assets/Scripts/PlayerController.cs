using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float pickUpRange = 5;
    private Rigidbody rb = null;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        bool moving = false;

        if (Input.GetButton("Horizontal"))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, -Input.GetAxis("Horizontal") * speed);
            moving = true;
        }

        if (Input.GetButton("Vertical"))
        {
            rb.velocity = new Vector3(Input.GetAxis("Vertical") * speed, 0 , rb.velocity.z);
            moving = true;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameController.ins.DropItem(GameController.ins.GetItemByID(Inventory.ins.items[0].id), new Vector2Int(Mathf.RoundToInt(GameController.mousePos.x), Mathf.RoundToInt(GameController.mousePos.y)), true);
            Inventory.ins.items.RemoveAt(0);
        }

        if (moving == false)
        {
            rb.velocity = Vector3.zero;
        }

        PickupInRadius();
    }

    void PickupInRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickUpRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "item")
            {
                hitCollider.GetComponent<ItemBehaviour>().Pickup();
            }
        }
    }
}
