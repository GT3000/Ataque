using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemies")]
    protected bool spawning;
    protected bool isSpawning;
    [SerializeField] protected List<Wave> waves;
    [SerializeField] protected float timeBetweenWaves;
    protected int currentWaveIndex;
    protected bool finalWave;

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
        if (currentWaveIndex <= waves.Count - 1)
        {
            if(waves[currentWaveIndex].currentEnemies > 0 && waves[currentWaveIndex].enemiesRemaining <= 0 && spawning)
            {
                spawning = false;
                StartCoroutine(CheckWave());
            }
        }

        if (finalWave && waves[currentWaveIndex].enemiesRemaining <= 0)
        {
            print("End mission");
            StopCoroutine(SpawnPowerups());
            StopCoroutine(SpawnEnemies());
            //TODO End Mission Animation
        }
    }

    private void BeginSpawner()
    {
        spawning = true;
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerups());
    }

    private IEnumerator SpawnEnemies()
    {
        if (waves[currentWaveIndex].currentEnemies < waves[currentWaveIndex].totalEnemiesInWave && spawning)
        {
            int randomIndex = Random.Range(0, waves[currentWaveIndex].enemies.Count);

            GameObject tempEnemy = Instantiate(waves[currentWaveIndex].enemies[randomIndex], transform.position, Quaternion.identity);
            tempEnemy.transform.parent = transform;
            waves[currentWaveIndex].currentEnemies++;
            waves[currentWaveIndex].enemiesRemaining++;

            yield return new WaitForSeconds(waves[currentWaveIndex].spawnInterval);
            
            StartCoroutine(SpawnEnemies());
        }

        yield return null;
    }

    private IEnumerator CheckWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        
        currentWaveIndex++;

        if (currentWaveIndex <= waves.Count && !finalWave)
        {
            if(waves[currentWaveIndex].isBoss)
            {
                finalWave = true;
                spawning = true;
                StartCoroutine(SpawnEnemies());
            }
        
            if (!waves[currentWaveIndex].isBoss)
            {
                spawning = true;
                StartCoroutine(SpawnEnemies());
            }
        }
    }
    
    // private IEnumerator SpawnEnemies()
    // {
    //     if (currentAmtSpawned < maxToSpawn && playerAlive && startSpawn)
    //     {
    //         int randomIndex = Random.Range(0, enemiesToSpawn.Length);
    //
    //         GameObject tempEnemy = Instantiate(enemiesToSpawn[randomIndex], transform.position, Quaternion.identity);
    //         tempEnemy.transform.parent = transform;
    //         currentAmtSpawned++;
    //         
    //         yield return new WaitForSeconds(spawnInterval);
    //
    //         StartCoroutine(SpawnEnemies());
    //     }
    //     else
    //     {
    //         spawning = false;
    //     }
    //
    //     yield return null;
    // }

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
                GameObject tempPowerup = Instantiate(powerupsToSpawn[i], transform.position, Quaternion.identity);
                tempPowerup.transform.parent = transform;

                yield return new WaitForSeconds(powerupSpawnInterval);
                
                StartCoroutine(SpawnPowerups());
                
                break;
            }
            else
            {
                randomIndex -= weightTable[i];
            }
        }
    }

    private void RemoveEnemies()
    {
        waves[currentWaveIndex].enemiesRemaining--;
    }

    private void StopSpawner()
    {
        spawning = false;
    }
}

[System.Serializable]
public class Wave
{
    public int totalEnemiesInWave;
    public int enemiesRemaining;
    public int currentEnemies;
    public float spawnInterval;
    public bool isBoss;
    public List<GameObject> enemies;
}
