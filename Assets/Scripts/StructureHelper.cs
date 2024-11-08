using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureHelper : MonoBehaviour
{
    public GameObject prefab;
    public Dictionary<Vector3Int, GameObject> structuresDictionary = new();


    public void PlaceStructures(List<Vector3Int> roadPositions)
    {
        Dictionary<Vector3Int , Direction> freeSpots = FindFreeSpaces(roadPositions);
        foreach (var position in freeSpots.Keys)
        {
            Instantiate(prefab, position, Quaternion.identity, transform);
        }
    }

    private Dictionary<Vector3Int, Direction> FindFreeSpaces(List<Vector3Int> roadPositions)
    {
        Dictionary<Vector3Int, Direction> freeSpaces = new();
        foreach (var position in roadPositions)
        {
            var neighborDirections = PlacementHelper.FindNeighbour(position, roadPositions);
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                if (!neighborDirections.Contains(direction))
                {
                    var newPos = position + PlacementHelper.GetOffsetFromDirection(direction);
                    if (freeSpaces.ContainsKey(newPos))
                    {
                        continue;
                    }
                    freeSpaces.Add(newPos, Direction.Right);
                }
            }
        }
        return freeSpaces;
    }
}
