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
    [SerializeField] protected float fireRate;
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected bool randomPattern;
    [SerializeField] protected bool recycle;

    protected GameObject projectileContainer;
    protected Vector3 screenBounds;
    protected float currentTime;
    
    // Start is called before the first frame update
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        
        projectileContainer = new GameObject(transform.name + "Projectile Container");

        RandomSpawnSpot(screenBounds);
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

        if (canFire && fireRate >= currentTime)
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
        if (col.CompareTag("Player") || col.CompareTag("Projectile"))
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
            Cleanup();
        }
    }

    private void Cleanup()
    {
        //TODO Pool enemies instead of destroying
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        
        Destroy(projectileContainer, 5.0f);
        Destroy(gameObject, 6.0f);
    }
}
