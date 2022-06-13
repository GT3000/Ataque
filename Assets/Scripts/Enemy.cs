using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int cashValue;
    [SerializeField] protected float speed;
    [SerializeField] protected bool canFire;
    [SerializeField] protected float minFireRate;
    [SerializeField] protected float maxFireRate;
    protected float currentFireRate;
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected bool randomPattern;
    [SerializeField] protected bool recycle;
    [SerializeField] protected GameObject deathFX;
    [SerializeField] protected AudioClip deathSfx;

    protected GameObject projectileContainer;
    protected Vector3 screenBounds;
    protected float currentTime;
    
    // Start is called before the first frame update
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        
        projectileContainer = new GameObject(transform.name + "Projectile Container");

        RandomSpawnSpot(screenBounds);
        RandomizeFireRate();
    }

    private void RandomSpawnSpot(Vector3 screenBounds)
    {
        if (randomPattern)
        {
            Vector3 randomPos = new Vector3(Random.Range(-screenBounds.x, screenBounds.x), screenBounds.y + 1, 0);

            transform.position = randomPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        transform.Translate(Vector3.down * speed *Time.deltaTime);

        if (canFire && currentFireRate <= currentTime)
        {
            if (projectile != null)
            {
                GameObject tempProjectile = Instantiate(projectile, transform.position, quaternion.identity);
                tempProjectile.transform.parent = projectileContainer.transform;
                currentTime = 0f;
            }
        }

        if (transform.position.y <= -1)
        {
            if (recycle)
            {
                RandomSpawnSpot(screenBounds);
            }
            else
            {
                Cleanup();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") || !col.GetComponent<Projectile>().EnemyProjectile)
        {
            if (col.CompareTag("Projectile"))
            {
                //TODO add score/cash to total
            }

            if (col.CompareTag("Player"))
            {
                //TODO destroy player and increment score/cash
                GameEvents.PlayerHit();
            }

            //TODO play enemy destruction VFX

            GameObject tempFx = Instantiate(deathFX, transform.position, Quaternion.identity);
            GameEvents.PlaySfx(deathSfx);
            
            Cleanup();
        }
    }

    private void RandomizeFireRate()
    {
        float randomFireRate = Random.Range(minFireRate, maxFireRate);
        currentFireRate = randomFireRate;
    }

    private void Cleanup()
    {
        if (canFire)
        {
            canFire = false;
        }
        
        //TODO Pool enemies instead of destroying
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        Destroy(projectileContainer, 5.0f);
        Destroy(gameObject, 6.0f);

        GameEvents.EnemyDestroyed();
        GameEvents.UpdateCash(cashValue);
    }
}
