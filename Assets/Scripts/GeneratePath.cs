using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneratePath : MonoBehaviour
{

    [Header("GenerationStatistics")] [SerializeField]
    private int NegativeY = 50;
    [SerializeField] private int PositiveY = 50;

    [SerializeField] private int PathWidth = 3;

    [Header("PathTiles")] [SerializeField] private GameObject PathTile;
    [SerializeField] private GameObject PathEdgeTile;
    [SerializeField] private GameObject VictoryTile;
    [SerializeField] private bool GeneratePathAtAll;

    [Header("ForestGeneration")] [SerializeField]
    private GameObject[] ForestFolliage;
    [SerializeField] private bool GenerateForestProc = false;
    [SerializeField] private int NumCastsToMake;
    [SerializeField] private Vector2 XBounds;
    [SerializeField] private Vector2 YBounds;

    [Header("Snow Generation")] [SerializeField]
    private List<GameObject> SnowObjects;
    [SerializeField] private int NumCastsToMakeSnow = 300;
    [SerializeField] private bool GenerateSnow = true;

    [Header("Animal Spawning")] [SerializeField]
    private bool SpawnAnimals = true;

    [SerializeField] private List<ForestAnimal> _animals;
    [SerializeField] private int NumRequiredAnimalsSpawned = 30;




    private int xOffset = 0;
    public 
    
    
    // Start is called before the first frame update
    void Start()
    {
        if(GeneratePathAtAll) GenerateNewPath();
        if (GenerateForestProc) GenerateForest();
        if (GenerateSnow) GenerateSnowProc();
        if (SpawnAnimals) SpawnAnimalsProc();
    }

    public void SpawnAnimalsProc()
    {
        Vector3 Origin;
        int numSuccesses = 0;
        while (numSuccesses < NumRequiredAnimalsSpawned)
        {
            float x = Random.Range(XBounds.x, XBounds.y);
            float y = Random.Range(YBounds.x, YBounds.y);
            Origin.x = x;
            Origin.y = y;
            Origin.z = 1;
            if (!Physics.Raycast(Origin, Vector3.down, 2f))
            {
                Origin.z = 0;
                int rdm = Random.Range(0, _animals.Count);
                GameObject newGameObj = Instantiate<GameObject>(_animals[rdm].gameObject, Origin, Quaternion.identity);
                
                newGameObj.transform.parent = this.gameObject.transform;
                numSuccesses++;
            }
        }
    }
    public void GenerateSnowProc()
    {
        Vector3 Origin;
        for (int i = 0; i < NumCastsToMakeSnow; i++)
        {
            float x = Random.Range(XBounds.x, XBounds.y);
            float y = Random.Range(YBounds.x, YBounds.y);
            Origin.x = x;
            Origin.y = y;
            Origin.z = 1;
            if (!Physics.Raycast(Origin, Vector3.down, 2f))
            {
                Origin.z = 0;
                int rdm = Random.Range(0, SnowObjects.Count);
                GameObject newGameObj = Instantiate<GameObject>(SnowObjects[rdm], Origin, Quaternion.identity);
                SetLayerBasedOnYValue(newGameObj);
                newGameObj.transform.parent = this.gameObject.transform;
            }
        }
    }

    private void SetLayerBasedOnYValue(GameObject obj)
    {
        Renderer[] r = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in r)
        {
            rend.sortingOrder = (int)obj.transform.position.y * -1;
        }
    }

    public void GenerateForest()
    {
        RaycastHit2D hit;
        Vector3 Origin;
        for (int i = 0; i < NumCastsToMake; i++)
        {
            float x = Random.Range(XBounds.x, XBounds.y);
            float y = Random.Range(YBounds.x, YBounds.y);
            Origin.x = x;
            Origin.y = y;
            Origin.z = 1;
            if (!Physics.Raycast(Origin, Vector3.down, 2f))
            {
                Origin.z = 0;
                int rdm = Random.Range(0, ForestFolliage.Length);
                GameObject newGameObj = Instantiate<GameObject>(ForestFolliage[rdm], Origin, Quaternion.identity);
                SetLayerBasedOnYValue(newGameObj);
                newGameObj.transform.parent = this.gameObject.transform;
            }
        }
    }

    public void GenerateNewPath()
    {
        for (int i = -NegativeY; i < PositiveY; i++)
        {
            if (Math.Abs(xOffset) == Math.Abs(i) && i < 0)
            {
                if (xOffset > 0)
                {
                    xOffset -= 1;
                }
                else
                {
                    xOffset += 1;
                }
            }
            else
            {
                int random = Random.Range(-1, 2);
                xOffset += random;
            }
            Instantiate(PathEdgeTile, new Vector3(-PathWidth -0.5f + xOffset, i+0.5f, 0f), Quaternion.identity);
            Instantiate(PathEdgeTile, new Vector3(PathWidth + 1.5f + xOffset, i+0.5f, 0f), Quaternion.identity);
            for (int j = -PathWidth; j <= PathWidth; j++)
            {
                Vector3 SpawnLoc = new Vector3(j + 0.5f + xOffset, i + 0.5f, 0f);
                Instantiate(PathTile, SpawnLoc, Quaternion.identity);
            }
        }

        for (int i = -PathWidth; i < PathWidth; i++)
        {
            Instantiate(VictoryTile, new Vector3(i + 0.5f + xOffset, PositiveY + 1.5f, 0f), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
