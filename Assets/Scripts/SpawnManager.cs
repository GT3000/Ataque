using System;
using System.Collections;
using System.Collections.Generic;
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

    private IEnumerator SpawnPowerups()
    {
        int randomIndex = Random.Range(0, powerupsToSpawn.Length);

        GameObject tempPowerup = Instantiate(powerupsToSpawn[randomIndex], transform.position, Quaternion.identity);
        tempPowerup.transform.parent = transform;

        yield return new WaitForSeconds(powerupSpawnInterval);

        StartCoroutine(SpawnPowerups());
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
