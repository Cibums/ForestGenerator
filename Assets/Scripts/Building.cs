using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building", fileName = "New Building")]
public class Building : ScriptableObject
{
    public string buildingName;
    public RotationType rotationType = RotationType.Free;
    public string[] tags = new string[0];
    public GameObject prefab;
    public bool fastPlacementOn;
    public bool destroyWhenPlacedOn = false;
    public int size = 1;
    [Range(0,0.5f)]
    public float randomCenterOffset = 0;
    public float randomLengthDifference = 0;
    public float randomColorOffset = 0;
    public Drop[] drops;
}

public enum RotationType
{
    Free,
    Frozen,
    RestrictedRandom,
    FullyRandom
}
