using UnityEngine;


[CreateAssetMenu(menuName = "ProceduralCity/Rule")]
public class Rule : ScriptableObject
{
    public string letter;

    [SerializeField]
    private string[] results = null;

    bool random = true;

    public string GetResults()
    {
        if (random)
        {
            int randomIndex = Random.Range(0, results.Length);
            return results[randomIndex];
        }
        else
        {
            return results[0];
        }
    }
}
