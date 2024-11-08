using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnManager : MonoBehaviour
{
    public GameObject buildingPrefab;
    public Material[] buildingMaterials;
    public int buildingCount = 10;
    //public float spacing = 10f; // Distance between buildings
    public Vector2 heightRange = new(1f, 10f);
    public float spawnDelay;

    IEnumerator SpawnBuildings()
    {
        List<Vector3> points = GeneratePoints();
        foreach (Vector3 point in GeneratePoints())
        {
            yield return StartCoroutine(PlaceBuilding(point));
        }
        Debug.Log("All buildings spawned");
    }

    public IEnumerator PlaceBuilding(Vector3 point)
    {
        yield return new WaitForSeconds(spawnDelay);
        Vector3 position = new(point.x, 0, point.z);
        GameObject building = Instantiate(buildingPrefab, position, Quaternion.identity);

        float randomHeight = Random.Range(heightRange.x, heightRange.y);
        building.transform.localScale = new Vector3(building.transform.localScale.x, randomHeight, building.transform.localScale.z);
        building.transform.position = new Vector3(building.transform.position.x, building.transform.localScale.y / 2, building.transform.position.z);
        building.GetComponent<MeshRenderer>().material = buildingMaterials[Random.Range(0, buildingMaterials.Length)];
    }

    public void Start()
    {
        StartCoroutine(SpawnBuildings());
    }

    List<Vector3> GeneratePoints()
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < buildingCount; i++)
        {
            float x = Random.Range(-100, 100);
            float z = Random.Range(-100, 100);
            if (points.Contains(new Vector3(x, 0, z))) continue;
            points.Add(new Vector3(x, 0, z));
        }
        return points;
    }
}