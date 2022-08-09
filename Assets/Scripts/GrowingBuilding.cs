using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Growing Building", fileName = "New Growing Building")]
public class GrowingBuilding : Building
{
    public Building nextStep;
    public int secondsMin;
    public int secondsMax;
}
