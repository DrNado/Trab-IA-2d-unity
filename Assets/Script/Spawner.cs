using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Spawner : MonoBehaviour
{
    public int amountSpawn;
    public float minX,maxX,minY,maxY;

    public static List<GameObject> initialNodesList = new List<GameObject>();
    public GameObject nodePrefab;
    public GameObject nodeHolder;

    void Awake()
    {
        for (int i = 0; i < amountSpawn; i++)
        {
            Vector2 placeToSpawn = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

            GameObject temp =  Instantiate(nodePrefab, placeToSpawn, Quaternion.identity, nodeHolder.transform); 
            initialNodesList.Add(temp);

            if (i == 0)
            {
                temp.GetComponent<SpriteRenderer>().color = Color.red;
            }

            temp.name = "City " + i.ToString();
        }
    }
}
