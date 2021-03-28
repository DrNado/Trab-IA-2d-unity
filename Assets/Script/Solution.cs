using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Solution 
{
    public List<GameObject> solutionInstance = new List<GameObject>();
    public float solutionWeight;

    public Solution(List<GameObject> instance, float weight)
    {
        solutionInstance = instance;
        solutionWeight = weight;
    }
}
