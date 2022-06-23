using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected int cashValue;
    [SerializeField] protected float speed;
    [SerializeField] protected bool randomPattern;
    [SerializeField] protected bool recycle;
    [SerializeField] protected GameObject deathFX;
    [SerializeField] protected AudioClip deathSfx;
    protected float defaultSpeed;
    [Header("Camera Shake")]
    [SerializeField] protected float shakeAmt;
    [SerializeField] protected float shakeSlopeOff;
    [SerializeField] protected float shakeTime;
    [Header("AI")]
    //Sideways Sweeping
    [SerializeField] protected bool sweeps;
    [SerializeField] protected float frequency;
    [SerializeField] protected float amplitude;
    protected Vector3 position;
    protected Vector3 axis;
    protected bool wasSweeping;
    //Ramming
    [SerializeField] protected bool rams;
    [SerializeField] protected float rammingSpeed;
    [SerializeField] protected float ramRange;
    [SerializeField] protected float lengthOfRam;
    protected bool isRamming;
    protected float rammingTimer;
    [Header("Projectiles")]
    //Firing Stats
    [SerializeField] protected bool canFire;
    [SerializeField] protected float minFireRate;
    [SerializeField] protected float maxFireRate;
    private bool directionSet;
    protected float currentFireRate;
    [SerializeField] protected GameObject projectile;
    [Header("Support")] 
    //Shield
    [SerializeField] protected bool shielded;
    [SerializeField] protected GameObject shield;
    
    [Header("Helper Variables")]
    [SerializeField] protected float screenBoundsOffset;
    protected Vector3 screenBounds;
    protected GameObject projectileContainer;
    protected float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        defaultSpeed = speed;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        projectileContainer = new GameObject(transform.name + "Projectile Container");

        RandomSpawnSpot(screenBounds);
        RandomizeFireRate();

        position = transform.position;
        axis = transform.right;
    }

    private void RandomSpawnSpot(Vector3 screenBounds)
    {
        if (randomPattern)
        {
            Vector3 randomPos = new Vector3(Random.Range(-screenBounds.x + screenBoundsOffset, screenBounds.x - screenBoundsOffset), screenBounds.y + 1, 0);

            transform.position = randomPos;
            position = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        Movement();

        Fire();

        Ram();
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, ramRange);

        if (hit.collider != null && rams && !isRamming)
        {
            if (hit.collider.CompareTag("Player"))
            {
                isRamming = true;
                rammingTimer = 0f;
            }
        }
        
        Debug.DrawRay(transform.position, Vector2.down, Color.red);
    }

    private void Fire()
    {
        if (canFire && currentFireRate <= currentTime)
        {
            if (projectile != null)
            {
                GameObject tempProjectile = Instantiate(projectile, transform.position, quaternion.identity);
                tempProjectile.transform.parent = projectileContainer.transform;
                currentTime = 0f;
            }
        }
    }

    private void Movement()
    {
        if (sweeps)
        {
            position += Vector3.down * Time.deltaTime * speed;
            transform.position = position + axis * Mathf.Sin(Time.time * frequency) * amplitude;
        }
        else
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }

        if (transform.position.y <= -1)
        {
            if (recycle)
            {
                if (rams)
                {
                    ResetRam();
                }
                
                RandomSpawnSpot(screenBounds);
            }
            else
            {
                Cleanup();
            }
        }
    }

    private void Ram()
    {
        if (isRamming)
        {
            rammingTimer += Time.deltaTime;
            speed = rammingSpeed;

            if (rammingTimer >= lengthOfRam)
            {
                ResetRam();
            }
        }
    }
    
    private void ResetRam()
    {
        isRamming = false;
        speed = defaultSpeed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") || col.CompareTag("Projectile"))
        {
            if (col.GetComponent<Projectile>() != null)
            {
                if (col.GetComponent<Projectile>().EnemyProjectile)
                {
                    //Ignore
                }
                else
                {
                    TakeDamage(col);
                }
            }
            
            if (col.CompareTag("Player"))
            {
                //TODO destroy player and increment score/cash
                GameEvents.PlayerHit();

                Cleanup();
            }
        }
    }

    private void TakeDamage(Collider2D col)
    {
        if (!shielded)
        {
            health -= col.GetComponent<Projectile>().Damage;

            if (health <= 0)
            {
                health = 0;
                Cleanup();
            }
        }
        else
        {
            shield.SetActive(false);
            shielded = false;
        }
    }

    private void RandomizeFireRate()
    {
        float randomFireRate = Random.Range(minFireRate, maxFireRate);
        currentFireRate = randomFireRate;
    }

    private void Cleanup()
    {
        GameObject tempFx = Instantiate(deathFX, transform.position, Quaternion.identity);
        GameEvents.PlaySfx(deathSfx);
        GameEvents.CameraShake(shakeAmt, shakeSlopeOff, shakeTime);
        
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
