using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystemGenerator : MonoBehaviour
{
    public Rule[] rules;
    public string rootSentence;
    
    [Range(0,10)] public int iterationLimit = 1;

    public bool randomIgnoreRuleModifier = true;
    [Range(0, 1)] public float chanceToIgnoreRule = 0.3f; 

    public string GenerateSentence(string word = null)
    {
        if (word == null)
        {
            word = rootSentence;
        }
        return GrowRecursive(word);
    }

    public string GrowRecursive(string word, int iterationIndex = 0)
    {
        if (iterationIndex >= iterationLimit)
        {
            return word;
        }
        StringBuilder newWord = new();

        foreach (var c in word)
        {
            newWord.Append(c);
            ProcessRules(newWord, c, iterationIndex);
        }

        return newWord.ToString();
    }

    private void ProcessRules(StringBuilder newWord, char c, int iterationIndex)
    {
        foreach (var rule in rules)
        {
            if (rule.letter == c.ToString())
            {
                if (randomIgnoreRuleModifier && iterationIndex > 1)
                {
                    if (UnityEngine.Random.value < chanceToIgnoreRule)
                    {
                        return;
                    }
                }
                newWord.Append(GrowRecursive(rule.GetResults(), iterationIndex + 1));
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GenerateSentence());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
