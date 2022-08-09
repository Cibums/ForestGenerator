using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlacedBuildingMetaData
{
    public bool isNull = false;
    public int buildingID;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Vector3 colorOffset;
}
