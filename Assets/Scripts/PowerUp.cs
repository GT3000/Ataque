using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum PowerupType
{
    Firepower,
    Energy,
    Missile,
    Shield,
    Speed,
    Health,
    Ammo,
    Immobilizer
}

public class PowerUp : MonoBehaviour
{
    [SerializeField] protected float lifetime;
    [SerializeField] protected int spawnWeight;
    [SerializeField] protected AudioClip pickupSfx;
    [SerializeField] protected PowerupType powerup;
    [SerializeField] protected float speed;
    [SerializeField] protected bool randomPattern;
    [SerializeField] protected bool negativePowerup;
    protected bool moveTowardsPlayer;
    protected Vector3 direction;
    protected Vector3 screenBounds;
    private float currentTime;
    
    public int SpawnWeight => spawnWeight;
    public bool NegativePowerup => negativePowerup;
    public bool MoveTowardsPlayer
    {
        get => moveTowardsPlayer;
        set => moveTowardsPlayer = value;
    }
    public Vector3 Direction
    {
        get => direction;
        set => direction = value;
    }

    private void OnEnable()
    {
        GameEvents.PowerupDestroyed += DestroyPowerup;
    }

    private void OnDisable()
    {
        GameEvents.PowerupDestroyed -= DestroyPowerup;
    }

    private void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        
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

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (moveTowardsPlayer)
        {
            MoveTowardDirection(direction);
        }
        else
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
        
        if (lifetime <= currentTime)
        {
            //TODO Pool instead of destroy
            Destroy(gameObject);
        }
    }

    private void MoveTowardDirection(Vector3 targetDirection)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetDirection, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameEvents.PowerupPickedUp((int)powerup);
            GameEvents.PlaySfx(pickupSfx);
            
            DestroyPowerup();
        }
    }

    //TODO Pool instead of destroy
    private void DestroyPowerup()
    {
        Destroy(gameObject);
    }
}
