using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlacedBuilding
{
    public Transform transform = null;
    public PlacedBuildingMetaData meta = new PlacedBuildingMetaData();

    public PlacedBuilding(Transform t, PlacedBuildingMetaData m)
    {
        transform = t;
        meta = m;
    }

    public PlacedBuilding()
    {
    }
}
