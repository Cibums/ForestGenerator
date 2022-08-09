using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedEntity
{
    public Transform transform;
    public EntityMetaData meta;

    public LoadedEntity(Transform t, EntityMetaData m)
    {
        transform = t;
        meta = m;
    }
}
