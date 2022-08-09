using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Growing : MonoBehaviour
{
    BuildingBehaviour bb;
    public float secondsPlaced = 0;
    private float secondsToWait = 600;

    private void Start()
    {
        try
        {
            bb = gameObject.GetComponent<BuildingBehaviour>();
            secondsToWait = Random.Range((bb.building as GrowingBuilding).secondsMin, (bb.building as GrowingBuilding).secondsMax);
        }
        catch
        {
            Debug.Log(" I AM DESTROYED ");
            Destroy(this);
        }
    }

    void Update()
    {
        if (bb != null)
        {
            float multiplier = GameController.ins.DoHaveSameContent(bb.building.tags, GameController.ins.TagsInSurroundingBuildings(GameController.ins.ToGridValue(transform.position))) ? 2 : 1;

            secondsPlaced += Time.deltaTime * multiplier;

            if (secondsPlaced >= secondsToWait)
            {
                Grow();
            }
        }
    }

    void Grow()
    {
        GameController.ins.PlaceBuilding((bb.building as GrowingBuilding).nextStep, transform.position, true, true);
    }
}
