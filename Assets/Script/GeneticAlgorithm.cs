using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class GeneticAlgorithm : MonoBehaviour
{
    public int worstWeightID;
    public int solutionListMaxSize;
    public int iterationAmount;
    public int finalIterationAmount;
    public int crossOverIterationAmount;
    public int bestSolution;
    public float chanceToMutation;
    public bool doNotUseAlwaysTwoBests;

    public List<Solution> finalSolutionList = new List<Solution>();
    public List<Solution> solutionList = new List<Solution>();

    public UnityEngine.UI.Text scoreText;

    void Start()
    {
        BuildInitialSolutionList();

       for(int i = 0; i < iterationAmount; i++)
        {
            RunGeneticAlgorithm();
        }

        FindBestSolution();
    }

    public void Reculculate()
    {
        solutionList.Clear();

        BuildInitialSolutionList();

        for (int i = 0; i < iterationAmount; i++)
        {
            RunGeneticAlgorithm();
        }

        FindBestSolution();
    }

    public void ResetFinalSolution()
    {
        bestSolution = 0;
        finalSolutionList.Clear();
        Reculculate();
    }
    public void BuildInitialSolutionList()
    {
        for(int j = 0; j < solutionListMaxSize; j++)
        {
            List<GameObject> tempList = FindInitialSolutionsPath();

            float tempScore = FindSolutionCost(tempList);

            Solution tempSolution = new Solution(tempList, tempScore);
            solutionList.Add(tempSolution);
        }
    }

    public List<GameObject> FindInitialSolutionsPath()
    {
        List<GameObject> tempList = new List<GameObject>();

        for (int i = 0; i < Spawner.initialNodesList.Count; i++)
        {
            tempList.Add(Spawner.initialNodesList[i]);
        }

        for (int i = 0; i < tempList.Count; i++)
        {
            int tempRandom1 = Random.Range(0, tempList.Count);
            GameObject temp1 = tempList[tempRandom1];
            int tempRandom2 = Random.Range(0, tempList.Count);
            GameObject temp2 = tempList[tempRandom2];
            if (tempRandom1 != 0 && tempRandom2 != 0)
            {
                tempList[tempRandom1] = temp2;
                tempList[tempRandom2] = temp1;
            }
        }
        return tempList;
    }

    public float FindSolutionCost(List<GameObject> tempList)
    {
        float totalCost = 0;

        for (int i = 0; i < Spawner.initialNodesList.Count; i++)
        {
            if(i < Spawner.initialNodesList.Count - 1) 
                totalCost += Vector2.Distance(tempList[i].transform.position, tempList[i + 1].transform.position);
        }

        totalCost += Vector2.Distance(tempList[tempList.Count - 1].transform.position, tempList[0].transform.position);

        return totalCost;
    }

    public void RunGeneticAlgorithm()
    {
        for (int i = 0; i < crossOverIterationAmount; i++)
        {
            RunCrossOver();

            if (Random.value < chanceToMutation) RunMutation();
        }

        FilterSolutionList();
    }

    public void RunCrossOver()
    {
        solutionList.Sort((x1,x2) => x1.solutionWeight.CompareTo(x2.solutionWeight));

        int listID1 = 0;
        int listID2 = 1;

        if(doNotUseAlwaysTwoBests)
        {
            listID1 = Random.Range(0, solutionListMaxSize);
            listID2 = Random.Range(0, solutionListMaxSize);
        }

        while (listID2 == listID1) listID2 = Random.Range(0, solutionListMaxSize);

        List<GameObject> tempList = new List<GameObject>();
        List<GameObject> secondaryList = new List<GameObject>();
        List<GameObject> primaryList = new List<GameObject>();

        for (int i = 0; i < Spawner.initialNodesList.Count; i++)
        {
            tempList.Add(solutionList[listID1].solutionInstance[i]);
            primaryList.Add(solutionList[listID1].solutionInstance[i]);
            secondaryList.Add(solutionList[listID2].solutionInstance[i]);
        }

        int listRandom;

        for (int i = 1; i < Spawner.initialNodesList.Count; i++)
        {
            listRandom = Random.Range(0, 2);

            if (listRandom == 0)
            {
                GameObject temp1 = primaryList[i];
                tempList[i] = temp1;

                int tempRandom2 = FindIndexOnList(secondaryList, temp1);
                GameObject temp2 = secondaryList[tempRandom2];
                GameObject temp3 = secondaryList[i];

                secondaryList[i] = temp2;
                secondaryList[tempRandom2] = temp3;
            }

            if (listRandom == 1)
            {
                GameObject temp1 = secondaryList[i];
                tempList[i] = temp1;

                int tempRandom2 = FindIndexOnList(primaryList, temp1);
                GameObject temp2 = primaryList[tempRandom2];
                GameObject temp3 = primaryList[i];

                primaryList[i] = temp2;
                primaryList[tempRandom2] = temp3;
            }
        }

        float tempScore = FindSolutionCost(tempList);

        Solution tempSolution = new Solution(tempList, tempScore);
        solutionList.Add(tempSolution);
    }

    public int FindIndexOnList(List<GameObject> list, GameObject objectToFind)
    {
        int index = 0;

        for(int i = 0; i <list.Count; i++)
        {
            if (list[i] == objectToFind)
                return i;
        }

        return index;
    }

    public void RunMutation()
    {
        int tempRandom1 = Random.Range(0, solutionList[solutionList.Count - 1].solutionInstance.Count);
        GameObject temp1 = solutionList[solutionList.Count - 1].solutionInstance[tempRandom1];
        int tempRandom2 = Random.Range(0, solutionList[solutionList.Count - 1].solutionInstance.Count);
        GameObject temp2 = solutionList[solutionList.Count - 1].solutionInstance[tempRandom2];

        if (tempRandom1 != 0 && tempRandom2 != 0 && temp1.gameObject.name != temp2.gameObject.name)
        {
            solutionList[solutionList.Count - 1].solutionInstance[tempRandom1] = temp2;
            solutionList[solutionList.Count - 1].solutionInstance[tempRandom2] = temp1;
        }

        float tempScore = FindSolutionCost(solutionList[solutionList.Count - 1].solutionInstance);

        Solution tempSolution = new Solution(solutionList[solutionList.Count - 1].solutionInstance, tempScore);
        solutionList.Remove(solutionList[solutionList.Count - 1]);
        solutionList.Add(tempSolution);
    }

    public void FilterSolutionList()
    {

        while(solutionList.Count > solutionListMaxSize)
        {
            int worstSolution = 0;

            for(int i = 1; i < solutionList.Count; i++)
            {
                if (solutionList[i].solutionWeight > solutionList[worstSolution].solutionWeight)
                    worstSolution = i;
            }
            solutionList.RemoveAt(worstSolution);
        }
    }

    public void FindBestSolution()
    {
        for (int i = 1; i < solutionList.Count; i++)
        {
            if (solutionList[i].solutionWeight < solutionList[bestSolution].solutionWeight)
                bestSolution = i;
        }

        finalSolutionList.Add(solutionList[bestSolution]);

        if (finalSolutionList.Count < finalIterationAmount)
        {
            Reculculate();
        }
        
        else
        {
            bestSolution = 0;
            for (int i = 1; i < finalSolutionList.Count; i++)
            {
                if (finalSolutionList[i].solutionWeight < finalSolutionList[bestSolution].solutionWeight)
                    bestSolution = i;
            }
            DrawLines();
        }
    }


    private void DrawLines()
    {
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.positionCount = finalSolutionList[bestSolution].solutionInstance.Count + 1;

        for (int i = 0; i < finalSolutionList[bestSolution].solutionInstance.Count; i++)
        {
            line.SetPosition(i, finalSolutionList[bestSolution].solutionInstance[i].transform.position);
            if(i != 0) finalSolutionList[bestSolution].solutionInstance[i].GetComponent<SpriteRenderer>().color = Color.blue;
        }

        finalSolutionList[bestSolution].solutionInstance[1].GetComponent<SpriteRenderer>().color = Color.green;
        finalSolutionList[bestSolution].solutionInstance[finalSolutionList[bestSolution].solutionInstance.Count - 1].GetComponent<SpriteRenderer>().color = Color.black;

        line.SetPosition(finalSolutionList[bestSolution].solutionInstance.Count, finalSolutionList[bestSolution].solutionInstance[0].transform.position);

        scoreText.text = "Solution Score: " + finalSolutionList[bestSolution].solutionWeight;

        finalSolutionList.Sort((x1, x2) => x1.solutionWeight.CompareTo(x2.solutionWeight));
    }
}
