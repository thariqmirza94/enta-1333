using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnitAI : UnitAI
{
    public override GridNode GetDestination()
    {
        GameObject targetGo = FindClosestWithTag("Enemy");
        CurrentTarget = targetGo; 

        if (targetGo == null) return null;

        GridNode targetNode = GridManager.GetNodeFromWorldPosition(targetGo.transform.position);
        if (targetNode.Walkable) return targetNode;

        GridNode myNode = GridManager.GetNodeFromWorldPosition(transform.position);
        GridNode fallback = GridManager.GetNearestWalkableNeighbour(myNode, targetNode);
        return fallback ?? targetNode;
    }

    GameObject FindClosestWithTag(string tag)
    {
        float min = float.MaxValue;
        GameObject nearest = null;
        foreach (var go in GameObject.FindGameObjectsWithTag(tag))
        {
            float d = Vector3.Distance(transform.position, go.transform.position);
            if (d < min) { min = d; nearest = go; }
        }
        return nearest;
    }
}