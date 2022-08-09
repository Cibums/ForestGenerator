using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed = 1;
    [HideInInspector]
    public Transform itemObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "item")
        {
            itemObject = other.transform;
            itemObject.gameObject.GetComponent<ItemBehaviour>().pickupable = false;
            itemObject.position = transform.position + new Vector3(0,0.5f,0);
            Destroy(itemObject.gameObject.GetComponent<Rigidbody>());
        }
    }

    private float t = 0;
    private void Update()
    {
        if (t >= speed)
        {
            Vector2Int pos = GameController.ins.ToGridValue(transform.position);

            if (GameController.ins.placedBuildings[pos.x + 1, pos.y] != null && GameController.ins.placedBuildings[pos.x + 1, pos.y].transform != null && GameController.ins.placedBuildings[pos.x + 1, pos.y].transform.gameObject.GetComponent<Conveyor>())
            {
                Transform t = GameController.ins.placedBuildings[pos.x + 1, pos.y].transform;
                Conveyor c = t.gameObject.GetComponent<Conveyor>();

                if (c.itemObject == null && itemObject != null)
                {
                    c.itemObject = itemObject;
                    itemObject.position = t.position + new Vector3(0, 0.5f, 0);
                    itemObject = null;
                }
            }

            t = 0;
        }

        t += Time.deltaTime;
    }
}
