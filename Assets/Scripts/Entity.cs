using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : ScriptableObject
{
    public string entityName;
    public GameObject prefab;
    public Drop[] killDrops;
    public AudioClip deathSound;
}
