using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : MonoBehaviour
{
    [HideInInspector]
    public Building building;

    private float t = 0;
    private void Update()
    {
        t += Time.deltaTime;

        if (t >= 3)
        {
            UnloadIfOutOfRange();
            t = 0;
        }
    }

    void UnloadIfOutOfRange()
    {
        float distance = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);

        if (distance >= GameController.ins.renderDistance * 1.3f)
        {
            Destroy(gameObject);
        }
    }

    public void DropItem()
    {
        Debug.Log("Dropped");
        //Destroy(gameObject);
    }
}
