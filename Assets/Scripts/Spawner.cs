using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
// using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEditor;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    //for collectibles of various sizes -- to be used when checking for overlapping area
    [SerializeField] public int[] sizeCheckLengths = {11, 5, 3};

    //for collectible prefabs that will be rand. selected based on avail space, decided by check lengths above.
    [Header("For all prefabs")]
    [SerializeField] private GameObject[] smallCollectiblesArray; 
    [SerializeField] private GameObject[] medCollectiblesArray;
    [SerializeField] private GameObject[] largeCollectiblesArray;
    [SerializeField] private GameObject[] airCollectiblesArray;

    //prefabs that will be spawned: obstacles, ground, platforms
    [SerializeField] private GameObject[] obstaclesToSpawn;
    [SerializeField] private GameObject groundUnitToSpawn;
    [SerializeField] private GameObject platformUnitToSpawn;
    [Space(10)]

    //list of all gameobjects alr spawned (needed to move all objects in GameManager script)
    [SerializeField] public List<GameObject> gameObjectsSpawned;

    //chances of spawning
    [SerializeField] private int chanceOfSpawningCollectible = 70;
    [SerializeField] private int platChance = 70;
    [SerializeField] private int spawnObjectChance = 70;

    // fields for checking if there is a hole
    [SerializeField] private float raycastLength;
    [SerializeField] private Transform raycastOrigin;

    [SerializeField] bool compPlatOff = false;

    // max holesize to switch off compulsory platforms above holes
    [SerializeField] private float toDisableCompPlat = 0.5f;

    [SerializeField] private float minHoleDel = 0.3f;
    [SerializeField] private float maxHoleDel = 1f;
    [SerializeField] private float defaultGroundDel = 0.5f;


    [SerializeField] private float spawnDelay;
    [SerializeField] private float airCollectiblesHeight = 3f;

    //toggles
    [Header("Toggle On/Off")]
    [SerializeField] public bool continuePlatforms = false; //platforms
    [SerializeField] public bool continueHoles = false; //ground
    [SerializeField] public bool continueObjects = false; // obstacles
    [SerializeField] public bool continueCollectibles = false; //collectibles
    [Space(10)]

    //all positions given
    [Header("All position arrays")]
    [SerializeField] private Transform[] platformPositions;
    [SerializeField] private Transform[] obstaclePositions;
    [SerializeField] private Transform[] collectiblePositions;
    [SerializeField] private Transform groundPosition;
    [Space(10)]

    [SerializeField] private float radiusToCheck = 3f; // for reachable platforms

    // to randomize spawn delays for platforms
    [SerializeField] private float highSpawnDelay = 3f;
    [SerializeField] private float medSpawnDelay = 1.5f;
    [SerializeField] private float lowSpawnDelay = 0.5f;
    
    private bool validPlatformSpawn;

    bool compulsoryPlat = false;
    bool spawningCompPlat = false;

    [SerializeField] float objectSpawnDel = 1f;
    [SerializeField] float areaCheckHeight = 0.3f;
    [SerializeField] float areaCheckLength = 5f;

    //for randomizing collectible spawnloop delays
    [SerializeField] float minCollectibleDel = 1f; 
    [SerializeField] float maxCollectibleDel = 2f; 


    // booleans for looping
    private bool spawningPlatform = false; //this is for platforms
    private bool generatingHoles = false;
    bool loopingObjSpawn = false;
    bool loopingCollectibles = false;

    // Update is called once per frame
    void Update()
    {
        // just to visualize the area to check before spawning in collectibles in the air
        UnityEngine.Debug.DrawLine(collectiblePositions[0].position + new Vector3(6, 5, 0), collectiblePositions[0].position + new Vector3(-6,-0.5f,0), Color.red);

        // check if theres a hole, then spawn in platform on lower layer unless hole is small (see SpawnHole fn)
        compulsoryPlat = !Physics2D.Raycast(raycastOrigin.position, Vector3.down, raycastLength);

        if(compulsoryPlat && continuePlatforms && !spawningCompPlat && !compPlatOff)
        {
            UnityEngine.Debug.DrawLine(raycastOrigin.position, raycastOrigin.position+(Vector3.down*raycastLength),Color.red);
            
            StartCoroutine(SpawnPlatform());
        }

        if(continuePlatforms && !spawningPlatform)
        {
            StartCoroutine(SpawnPlatform());
        }


        if(continueHoles && !generatingHoles)
        {
            StartCoroutine(SpawnHole());
        }

        if(continueObjects && !loopingObjSpawn)
        {
            StartCoroutine(ObjSpawnLoop());
        }

        if(continueCollectibles && !loopingCollectibles)
        {
            StartCoroutine(CollectibleSpawnLoop());
        }
    }

    private void SpawnRandCollectible(GameObject[] array, Vector3 position)
    {
        GameObject collectibleToSpawn = array[Random.Range(0, array.Length)];
        GameObject spawnedCollectible = Instantiate(collectibleToSpawn, position, Quaternion.identity);
        gameObjectsSpawned.Add(spawnedCollectible);
    }

    private IEnumerator CollectibleSpawnLoop()
    {
        loopingCollectibles = true;
        // if area reachable by jumping has a super large empty space, 50% chance to spawn smth in air.
        if(!Physics2D.OverlapArea(collectiblePositions[0].position + new Vector3(6, 5, 0), collectiblePositions[0].position + new Vector3(-6,-0.5f,0)))
        {
            if(Random.Range(0, 2)==1)
            {
                SpawnRandCollectible(airCollectiblesArray, collectiblePositions[0].position + new Vector3 (0, airCollectiblesHeight, 0));
                UnityEngine.Debug.Log("air c. spawned");
            }
        }

        //checking all positions
        foreach(Transform spawnPoint in collectiblePositions)
        {
            //going through all lengths
            foreach(int length in sizeCheckLengths)
            {
                //Checking if theres a floor - if raycasting both ends hit something within small range
                bool leftHit = Physics2D.Raycast(spawnPoint.position + new Vector3(-length/2, 0, 0), Vector2.down, 1f);
                bool rightHit = Physics2D.Raycast(spawnPoint.position + new Vector3(length/2, 0, 0), Vector2.down, 1f);
                bool midHit = Physics2D.Raycast(spawnPoint.position, Vector2.down, 1f);
                bool hasGroundBelow = leftHit && rightHit && midHit ? true : false;

                UnityEngine.Debug.DrawLine(spawnPoint.position + new Vector3(-length/2, 0, 0), spawnPoint.position + new Vector3(-length/2, 0, 0) + new Vector3(0, -1f, 0), Color.red);
                UnityEngine.Debug.DrawLine(spawnPoint.position + new Vector3(length/2, 0, 0), spawnPoint.position + new Vector3(length/2, 0, 0) + new Vector3(0, -1f, 0), Color.red);
                
                //Then check if theres overlapping obstacle.
                bool noOverlap = !Physics2D.OverlapArea(spawnPoint.position + new Vector3(-length/2,-areaCheckHeight/2,0), spawnPoint.position + new Vector3(length/2,areaCheckHeight/2,0));

                //If both true, valid spawn point. Spawn something within the corresponding array (switch)
                if(noOverlap && hasGroundBelow)
                {
                    //Decide whether it will spawn in this valid position (by chance)
                    if(Random.Range(0, 101) <= chanceOfSpawningCollectible)
                    {
                        //Spawning objects based on size available
                        if(Array.IndexOf(sizeCheckLengths, length) == 0)
                        {
                            SpawnRandCollectible(largeCollectiblesArray, spawnPoint.position);
                            UnityEngine.Debug.Log($"Large spawned at {spawnPoint}");
                        }
                        else if(Array.IndexOf(sizeCheckLengths, length) == 1)
                        {
                            SpawnRandCollectible(medCollectiblesArray, spawnPoint.position);
                            UnityEngine.Debug.Log($"Med spawned at {spawnPoint}");
                        }
                        else
                        {
                            SpawnRandCollectible(smallCollectiblesArray, spawnPoint.position);
                            UnityEngine.Debug.Log($"Small spawned at {spawnPoint}");
                        }
                    }
                }
            }

        }

        yield return new WaitForSeconds(Random.Range(minCollectibleDel, maxCollectibleDel));
        loopingCollectibles = false;
    }

    private IEnumerator SpawnHole()
    {
        generatingHoles = true;

        int holeChanceNum = Random.Range(0,101);
        float randHoleSize = 0;

        if(holeChanceNum <= 10)
        {
            // have spawngap rand duration
            randHoleSize = Random.Range(minHoleDel, maxHoleDel);
            if (randHoleSize < toDisableCompPlat)
            {
                StartCoroutine(SetCompOff(randHoleSize + 0.5f));
            }
        }

        else
        {
            //spawn 1 p large unit
            GameObject spawnedGround = Instantiate(groundUnitToSpawn, groundPosition.position, Quaternion.identity);
            gameObjectsSpawned.Add(spawnedGround);
        }

        // every 0.5s, loop this
        yield return new WaitForSeconds(defaultGroundDel + randHoleSize);
        generatingHoles = false;
    }

    private IEnumerator SetCompOff(float delay)
    {
        compPlatOff = true;
        yield return new WaitForSeconds(delay);
        compPlatOff = false;
    }

    private void SpawnObject(Transform spawnPoint)
    {
        UnityEngine.Debug.Log("Object Spawned");
        GameObject objectToSpawn = obstaclesToSpawn[Random.Range(0, obstaclesToSpawn.Length)];
        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPoint.position, Quaternion.identity);
        gameObjectsSpawned.Add(spawnedObject);

    }

    private IEnumerator ObjSpawnLoop()
    {
        loopingObjSpawn = true;

        // check there is ground below & there is no entity overlapping within a box range
        foreach(Transform spawnPoint in obstaclePositions)
        {
            bool leftHit = Physics2D.Raycast(spawnPoint.position + new Vector3(-1.5f, 0, 0), Vector2.down, 1f);
            bool rightHit = Physics2D.Raycast(spawnPoint.position + new Vector3(1.5f, 0, 0), Vector2.down, 1f);
            bool hasGroundBelow = leftHit && rightHit ? true : false;
            bool noOverlap = !Physics2D.OverlapArea(spawnPoint.position + new Vector3(-areaCheckLength/2,-areaCheckHeight/2,0), spawnPoint.position + new Vector3(areaCheckLength/2,areaCheckHeight/2,0));

            if (hasGroundBelow && noOverlap)
            {
                float randNum = Random.Range(0, 101);
                if(randNum <= spawnObjectChance)
                {
                    SpawnObject(spawnPoint);
                }   
            }

            else
            {
                UnityEngine.Debug.Log($"Invalid Spawn at {spawnPoint.gameObject.name}");
            }
        }     

        yield return new WaitForSeconds(objectSpawnDel);
        loopingObjSpawn = false;
    }

    private IEnumerator SpawnPlatform()
    {

            spawningPlatform = true;
            Transform platformPosition = platformPositions[Random.Range(0, platformPositions.Length)];
            validPlatformSpawn = Physics2D.OverlapCircle(platformPosition.position, radiusToCheck);

            if(validPlatformSpawn && !compulsoryPlat)
                {
                    // set ur chances
                    if(Random.Range(0,101) <= platChance)
                    {
                        GameObject objectToSpawn = platformUnitToSpawn;
                        GameObject spawnedObject = Instantiate(objectToSpawn, platformPosition.position, Quaternion.identity);
                        gameObjectsSpawned.Add(spawnedObject);
                    }
                }       

            if(compulsoryPlat)
            {
                spawningCompPlat = true;
                platformPosition = platformPositions[0];

                GameObject objectToSpawn = platformUnitToSpawn;
                GameObject spawnedObject = Instantiate(objectToSpawn, platformPosition.position, Quaternion.identity);
                gameObjectsSpawned.Add(spawnedObject);
                spawnDelay = 0.55f;
            }

            else
            {
                int randNum = Random.Range(0,101);

                if(randNum <= 98)
                {
                    spawnDelay = lowSpawnDelay;
                }

                else if(randNum == 100)
                {
                    spawnDelay = highSpawnDelay;
                }

                else
                {
                    spawnDelay = medSpawnDelay;
                }
            }

        yield return new WaitForSeconds(spawnDelay);
        spawningPlatform = false;
        spawningCompPlat = false;
    }

}
