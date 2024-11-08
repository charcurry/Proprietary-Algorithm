using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SimpleVisualizer;

public class Visualizer : MonoBehaviour
{
    public LSystemGenerator lSystem;

    public RoadHelper roadHelper;
    public StructureHelper structureHelper;
    public int roadLength = 8;
    private int length = 8;
    private float angle = 90;

    public int Length
    {
        get
        {
            if (length > 0)
            {
                return length;
            }
            else
            {
                return 1;
            }
        }
        set => length = value;
    }

    public void Start()
    {
        CreateTown();
    }

    public void CreateTown()
    {
        length = roadLength;
        roadHelper.Reset();
        structureHelper.Reset();
        var sequence = lSystem.GenerateSentence();
        VisualSequence(sequence);
    }

    private void VisualSequence(string sequence)
    {
        Stack<AgentParameters> savePoints = new();
        var currentPosition = Vector3.zero;

        Vector3 direction = Vector3.forward;
        Vector3 tempPosition = Vector3.zero;


        foreach (var letter in sequence)
        {
            EncodingLetters encoding = (EncodingLetters)letter;
            switch (encoding)
            {
                case EncodingLetters.unknown:
                    break;
                case EncodingLetters.save:
                    savePoints.Push(new AgentParameters
                    {
                        direction = direction,
                        length = Length
                    });
                    break;
                case EncodingLetters.load:
                    if (savePoints.Count > 0)
                    {
                        var agentParameter = savePoints.Pop();
                        currentPosition = agentParameter.position;
                        direction = agentParameter.direction;
                        Length = agentParameter.length;
                    }
                    else
                    {
                        throw new System.Exception("Don't have active save point");
                    }
                    break;
                case EncodingLetters.draw:
                    tempPosition = currentPosition;
                    currentPosition += direction * length;
                    roadHelper.PlaceStreetPositions(tempPosition, Vector3Int.RoundToInt(direction), length);
                    Length -= 2;
                    break;
                case EncodingLetters.turnRight:
                    direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                    break;
                case EncodingLetters.turnLeft:
                    direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                    break;
                default:
                    break;
            }
        }
        roadHelper.FixRoad();
        structureHelper.PlaceStructures(roadHelper.GetRoadPositions());
    }
}