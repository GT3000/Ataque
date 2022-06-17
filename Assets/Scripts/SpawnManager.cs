using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] protected GameObject[] enemiesToSpawn;
    [SerializeField] protected float spawnInterval;
    [SerializeField] protected int maxToSpawn;
    [SerializeField] protected int currentAmtSpawned;
    protected bool spawning;
    protected bool playerAlive = true;
    protected bool startSpawn;

    [Header("Powerups")] 
    [SerializeField] protected GameObject[] powerupsToSpawn;
    [SerializeField] protected float powerupSpawnInterval;
    [SerializeField] protected List<int> weightTable;

    private void OnEnable()
    {
        GameEvents.EnemyDestroyed += RemoveEnemies;
        GameEvents.PlayerDestroyed += StopSpawner;
        GameEvents.SpawningStarted += BeginSpawner;
    }

    private void OnDisable()
    {
        GameEvents.EnemyDestroyed -= RemoveEnemies;
        GameEvents.PlayerDestroyed -= StopSpawner;
        GameEvents.SpawningStarted -= BeginSpawner;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawning && currentAmtSpawned < maxToSpawn && startSpawn)
        {
            spawning = true;

            StartCoroutine(SpawnEnemies());
        }
    }

    private void BeginSpawner()
    {
        startSpawn = true;
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerups());
    }

    private IEnumerator SpawnEnemies()
    {
        if (currentAmtSpawned < maxToSpawn && playerAlive && startSpawn)
        {
            int randomIndex = Random.Range(0, enemiesToSpawn.Length);

            GameObject tempEnemy = Instantiate(enemiesToSpawn[randomIndex], transform.position, Quaternion.identity);
            tempEnemy.transform.parent = transform;
            currentAmtSpawned++;
            
            yield return new WaitForSeconds(spawnInterval);

            StartCoroutine(SpawnEnemies());
        }
        else
        {
            spawning = false;
        }

        yield return null;
    }

    // private IEnumerator SpawnPowerups()
    // {
    //     int randomIndex = Random.Range(0, powerupsToSpawn.Length);
    //
    //     GameObject tempPowerup = Instantiate(powerupsToSpawn[randomIndex], transform.position, Quaternion.identity);
    //     tempPowerup.transform.parent = transform;
    //
    //     yield return new WaitForSeconds(powerupSpawnInterval);
    //
    //     StartCoroutine(SpawnPowerups());
    // }

    private IEnumerator SpawnPowerups()
    {
        int totalWeight = 0;
        int randomIndex;
        
        foreach (var powerup in powerupsToSpawn)
        {
            totalWeight += powerup.GetComponent<PowerUp>().SpawnWeight;
            weightTable.Add(powerup.GetComponent<PowerUp>().SpawnWeight);
        }

        randomIndex = Random.Range(0, totalWeight);

        for (int i = 0; i < weightTable.Count; i++)
        {
            if (randomIndex <= weightTable[i])
            {
                print("Weight is: " + randomIndex);
                
                GameObject tempPowerup = Instantiate(powerupsToSpawn[i], transform.position, Quaternion.identity);
                tempPowerup.transform.parent = transform;

                yield return new WaitForSeconds(powerupSpawnInterval);
                
                StartCoroutine(SpawnPowerups());
                
                break;
            }
            else
            {
                randomIndex -= weightTable[i];
                
                print("Reached here");
            }
        }
    }

    private void RemoveEnemies()
    {
        currentAmtSpawned--;
    }

    private void StopSpawner()
    {
        playerAlive = false;
        print("Player died. Stop spawning.");
    }
}
