using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBehaviour : MonoBehaviour
{
    public Entity entity;

    public virtual void Update()
    {
        if (Vector3.Distance(GameController.ins.player.position, transform.position) > GameController.ins.renderDistance)
        {
            EntityMetaData meta = new EntityMetaData();
            meta.entityID = GameController.ins.GetEntityID(entity);
            meta.position = transform.position;
            meta.rotation = transform.rotation;

            GameController.ins.loadedEntities.Remove(transform);
            GameController.ins.unloadedEntities.Add(meta);

            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        
    }

    public void Die()
    {
        
    }
}
