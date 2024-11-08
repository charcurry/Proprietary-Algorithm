using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureHelper : MonoBehaviour
{
    public HouseType[] buildingTypes;
    public Dictionary<Vector3Int, GameObject> structuresDictionary = new();


    public void PlaceStructures(List<Vector3Int> roadPositions)
    {
        Dictionary<Vector3Int, Direction> freeSpots = FindFreeSpaces(roadPositions);
        List<Vector3Int> blockedPositions = new();
        foreach (var freeSpot in freeSpots)
        {
            if (blockedPositions.Contains(freeSpot.Key))
            {
                continue;
            }
            var rotation = Quaternion.identity;
            switch (freeSpot.Value)
            {
                case Direction.Up:
                    rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.Down:
                    rotation = Quaternion.Euler(0, -180, 0);
                    break;
                case Direction.Right:
                    rotation = Quaternion.Euler(0, 180 - 90, 0);
                    break;
            }
            for (var i = 0; i < buildingTypes.Length; i++)
            {
                if (buildingTypes[i].quantity == -1)
                {
                    var building = SpawnPrefab(buildingTypes[i].GetPrefab(), freeSpot.Key, rotation);
                    structuresDictionary.Add(freeSpot.Key, building);
                    break;
                }
                if (buildingTypes[i].IsBuildingAvailable())
                {
                    if (buildingTypes[i].sizeRequired > 1)
                    {
                        var halfSize = Mathf.FloorToInt(buildingTypes[i].sizeRequired / 2.0f);
                        List<Vector3Int> tempPositionsLocked = new();
                        if (VerifyIfBuildingFits(halfSize, freeSpots, freeSpot, blockedPositions, ref tempPositionsLocked))
                        {
                            blockedPositions.AddRange(tempPositionsLocked);
                            var building = SpawnPrefab(buildingTypes[i].GetPrefab(), freeSpot.Key, rotation);
                            structuresDictionary.Add(freeSpot.Key, building);
                            foreach (var pos in tempPositionsLocked)
                            {
                                structuresDictionary.Add(pos, building);
                            }
                        }
                    }
                    else
                    {
                        var building = SpawnPrefab(buildingTypes[i].GetPrefab(), freeSpot.Key, rotation);
                        structuresDictionary.Add(freeSpot.Key, building);
                    }
                    break;
                }
            }
        }
    }

    private bool VerifyIfBuildingFits(int halfSize, Dictionary<Vector3Int, Direction> freeSpots, KeyValuePair<Vector3Int, Direction> freeSpot, List<Vector3Int> blockedPositions, ref List<Vector3Int> tempPositionsLocked)
    {
        Vector3Int direction = Vector3Int.zero;
        if (freeSpot.Value == Direction.Down || freeSpot.Value == Direction.Up)
        {
            direction = Vector3Int.right;
        }
        else
        {
            direction = new Vector3Int(0,0,1);
        }
        for(int i = 1; i < halfSize; i++)
        {
            var pos1 = freeSpot.Key + direction * i;
            var pos2 = freeSpot.Key - direction * i;
            if (!freeSpots.ContainsKey(pos1) || !freeSpots.ContainsKey(pos2) || blockedPositions.Contains(pos1) || blockedPositions.Contains(pos2))
            {
                return false;
            }
            tempPositionsLocked.Add(pos1);
            tempPositionsLocked.Add(pos2);
        }
        return true;
    }

    private GameObject SpawnPrefab(GameObject prefab, Vector3Int position, Quaternion rotation)
    {
        var newStructure = Instantiate(prefab, position, rotation, transform);
        return newStructure;
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
                    freeSpaces.Add(newPos, PlacementHelper.GetReverseDirection(direction));
                }
            }
        }
        return freeSpaces;
    }

    public void Reset()
    {
        foreach (var item in structuresDictionary.Values)
        {
            Destroy(item);
        }
        structuresDictionary.Clear();

        foreach (var buildingType in buildingTypes)
        {
            buildingType.Reset();
        }
    }
}
