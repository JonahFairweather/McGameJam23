using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneratePath : MonoBehaviour
{

    [Header("GenerationStatistics")] [SerializeField]
    private int NegativeY = 50;
    [SerializeField] private int PositiveYMax= 500;

    [SerializeField] private int PathWidth = 3;

    [Header("PathTiles")] [SerializeField] private GameObject PathTile;
    [SerializeField] private GameObject PathEdgeTile;
    [SerializeField] private GameObject VictoryTile;
    [SerializeField] private bool GeneratePathAtAll;

    [SerializeField] private GameObject PenguinToSpawn;

    [Header("ForestGeneration")] [SerializeField]
    private GameObject[] ForestFolliage;
    [SerializeField] private bool GenerateForestProc = false;
    [SerializeField] private int NumCastsToMakePerYLevel = 25;
    [SerializeField] private Vector2 XBounds;
    [SerializeField] private Vector2 YBounds;

    [Header("Snow Generation")] [SerializeField]
    private List<GameObject> SnowObjects;
    [SerializeField] private int NumCastsPerYLevel = 50;
    [SerializeField] private bool GenerateSnow = true;
    [SerializeField] private Transform FollowTransform;
    private int YThreshold = 0;

    [Header("Animal Spawning")] [SerializeField]
    private bool SpawnAnimals = true;

    [SerializeField] private List<ForestAnimal> _animals;
    [SerializeField] private int NumSuccessesPerYLevel = 3;

    protected Queue<GameObject> _spawnedAnimals;
    protected Queue<GameObject> _snowObjects;
    protected Queue<GameObject> _forestObjects;
    
    




    private int xOffset = 0;


    private void Update()
    {
        if (FollowTransform)
        {
            if (Mathf.FloorToInt(FollowTransform.position.y) > YThreshold)
            {
                YThreshold++;
                if (YThreshold == 450)
                {
                    SpawnPenguinFamily();
                }
                Debug.Log(YThreshold);
                GenerateStuffs(YThreshold + 50);
            }
        }
    }

    private void SpawnPenguinFamily()
    {
        for (int i = -100; i < 101; i+=2)
        {
            for (int j = 500; j < 510; i += 10)
            {
                float xOffset = Random.Range(-10, 10);
                float yOffset = Random.Range(-2, 2);
                Vector3 Loc = new Vector3(i + xOffset, j + yOffset, 0);
                GameObject g = Instantiate(PenguinToSpawn, Loc, Quaternion.identity);
                if (g != null)
                {
                    g.GetComponent<SpriteRenderer>().flipX = Random.Range(0, 2) == 1;
                }
            }
        }
    }
    
    private void GenerateStuffs(int YValue)
    {
        Vector3 Origin = new Vector3();
        
        if (YValue % 3 == 0)
        {
            int successes = 0;
            while (successes < NumSuccessesPerYLevel)
            {
                float YOffset = Random.Range(-1, 1);
                float XValue = Random.Range(XBounds.x, XBounds.y);
                Origin.x = XValue;
                Origin.y = YValue + YOffset;
                Origin.z = 1f;
                if (!Physics.Raycast(Origin, Vector3.down, 2f)){
                    Origin.z = 0;
                    int rdm = Random.Range(0, _animals.Count);
                    GameObject newGameObj = Instantiate<GameObject>(_animals[rdm].gameObject, Origin, Quaternion.identity);
                    _spawnedAnimals.Enqueue(newGameObj);
                    
                    GameObject lastAnimal = _spawnedAnimals.Dequeue();
                    
                    Destroy(lastAnimal);
                    SetLayerBasedOnYValue(newGameObj);
                    newGameObj.transform.parent = this.gameObject.transform;
                    successes++;
                    
                }
            }
        }
        int casts = 0;
        while (casts < NumCastsToMakePerYLevel)
        {
            float x = Random.Range(XBounds.x, XBounds.y);
            float yOffset = Random.Range(-1, 1);
            Origin.x = x;
            Origin.y = YValue + yOffset;
            Origin.z = 1;
            if (!Physics.Raycast(Origin, Vector3.down, 2f))
            {
                Origin.z = 0;
                int rdm = Random.Range(0, ForestFolliage.Length);
                GameObject newGameObj =
                    Instantiate<GameObject>(ForestFolliage[rdm].gameObject, Origin, Quaternion.identity);
                if (newGameObj != null)
                {
                    
                    _forestObjects.Enqueue(newGameObj);
                    SetLayerBasedOnYValue(newGameObj);
                    newGameObj.transform.parent = this.gameObject.transform;
                }

                GameObject lastForest = _forestObjects.Dequeue();
                while (lastForest == null)
                {
                    lastForest = _forestObjects.Dequeue();
                }
            }

            casts++;
        }
        casts = 0;
        while (casts < NumCastsPerYLevel)
        {
            float x = Random.Range(XBounds.x, XBounds.y);
            float yOffset = Random.Range(-1, 1);
            Origin.x = x;
            Origin.y = YValue + yOffset;
            Origin.z = 1;
            if (!Physics.Raycast(Origin, Vector3.down, 2f))
            {
                Origin.z = 0;
                int rdm = Random.Range(0, SnowObjects.Count);
                GameObject newGameObj = Instantiate<GameObject>(SnowObjects[rdm].gameObject, Origin, Quaternion.identity);
                _snowObjects.Enqueue(newGameObj);
                SetLayerBasedOnYValue(newGameObj);
                newGameObj.transform.parent = this.gameObject.transform;
                GameObject lastSnow = _snowObjects.Dequeue();
                while (lastSnow == null)
                {
                    lastSnow = _forestObjects.Dequeue();
                }
            }

            casts++;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        YThreshold = (int)FollowTransform.position.y;
        _forestObjects = new Queue<GameObject>();
        _spawnedAnimals = new Queue<GameObject>();
        _snowObjects = new Queue<GameObject>();
        
        if (GenerateForestProc) SpawnInitialForest();
        if (GenerateSnow) SpawnInitialSnow();
        if (SpawnAnimals) SpawnInitialAnimals();
    }

    public void SpawnInitialForest()
    {
        Vector3 Origin;
        for (int i = -50; i <= 50; i++)
        {
            int casts = 0;
            while (casts < NumCastsToMakePerYLevel)
            {
                float x = Random.Range(XBounds.x, XBounds.y);
                float yOffset = Random.Range(-1, 1);
                Origin.x = x;
                Origin.y = i + yOffset;
                Origin.z = 1;
                if (!Physics.Raycast(Origin, Vector3.down, 2f))
                {
                    Origin.z = 0;
                    int rdm = Random.Range(0, ForestFolliage.Length);
                    GameObject newGameObj = Instantiate<GameObject>(ForestFolliage[rdm].gameObject, Origin, Quaternion.identity);
                    if (newGameObj != null)
                    {
                        Debug.Log(_forestObjects);
                        _forestObjects.Enqueue(newGameObj);
                        SetLayerBasedOnYValue(newGameObj);
                        newGameObj.transform.parent = this.gameObject.transform;
                    }
                }

                casts++;
            }
        }
       
        
    }
    public void SpawnInitialSnow()
    {
        Vector3 Origin;
        for (int i = -50; i < NumCastsPerYLevel; i++)
        {
            int casts = 0;
            while (casts < NumCastsPerYLevel)
            {
                float x = Random.Range(XBounds.x, XBounds.y);
                float yOffset = Random.Range(-1, 1);
                Origin.x = x;
                Origin.y = i + yOffset;
                Origin.z = 1;
                if (!Physics.Raycast(Origin, Vector3.down, 2f))
                {
                    Origin.z = 0;
                    int rdm = Random.Range(0, SnowObjects.Count);
                    GameObject newGameObj = Instantiate<GameObject>(SnowObjects[rdm].gameObject, Origin, Quaternion.identity);
                    _snowObjects.Enqueue(newGameObj);
                    SetLayerBasedOnYValue(newGameObj);
                    newGameObj.transform.parent = this.gameObject.transform;
                }

                casts++;
            }
        }
    }
    public void SpawnInitialAnimals()
    {
        Vector3 Origin;
        for (int i = -50; i <= 50; i+=5)
        {
            int numSuccesses = 0;
            while (numSuccesses < NumSuccessesPerYLevel)
            {
                float x = Random.Range(XBounds.x, XBounds.y);
                float yOffset = Random.Range(-1, 1);
                Origin.x = x;
                Origin.y = i + yOffset;
                Origin.z = 1;
                if (!Physics.Raycast(Origin, Vector3.down, 2f))
                {
                    Origin.z = 0;
                    int rdm = Random.Range(0, _animals.Count);
                    GameObject newGameObj = Instantiate<GameObject>(_animals[rdm].gameObject, Origin, Quaternion.identity);
                    _spawnedAnimals.Enqueue(newGameObj);
                    SetLayerBasedOnYValue(newGameObj);
                    newGameObj.transform.parent = this.gameObject.transform;
                    numSuccesses++;
                }
            }
        }
    }
   
    public void GenerateSnowProc()
    {
        Vector3 Origin;
        for (int i = 0; i < NumCastsPerYLevel; i++)
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
    
}
