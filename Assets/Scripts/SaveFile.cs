using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    public Settings settings;
    public Quaternion sunRotation;
    public int worldSize;
    public Vector3 playerPosition;
    public Pixel[] groundTexture;
    public PlacedBuildingMetaData[] buildingData;
    public EntityMetaData[] entityData;
    public ItemMetaData[] itemData;
    public InventoryItem[] inventory;
}

[System.Serializable]
public class Settings
{

}

[System.Serializable]
public class Pixel
{
    public float r;
    public float g;
    public float b;
}